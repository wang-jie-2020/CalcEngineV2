﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Mime;
using CalcEngine.Expressions;
using CalcEngine.Functions;

namespace CalcEngine
{
    /// <summary>
    /// CalcEngine parses strings and returns Expression objects that can 
    /// be evaluated.
    /// </summary>
    /// <remarks>
    /// <para>This class has three extensibility points:</para>
    /// <para>Use the <b>DataContext</b> property to add an object's properties to the engine scope.</para>
    /// <para>Use the <b>RegisterFunction</b> method to define custom functions.</para>
    /// <para>Override the <b>GetExternalObject</b> method to add arbitrary variables to the engine scope.</para>
    /// </remarks>
    public partial class CalcEngine
    {
        #region ** fields

        //本地化
        CultureInfo _ci;
        char _decimal, _listSep, _percent;

        //前缀忽略字符
        string _idChars;

        //引擎表达式缓存
        CalcExpressionCache _cache;

        //已注册功能
        Dictionary<string, FunctionDefinition> _fns;

        //符号集,+-*/
        Dictionary<object, Token> _tks;

        // members
        string _expr;                       // expression being parsed
        int _len;                       // length of the expression being parsed
        int _ptr;                       // current pointer into expression

        Token _token;                       // current token being parsed

        #endregion

        #region ** token/keyword tables

        // build/get static token table
        Dictionary<object, Token> GetSymbolTable()
        {
            if (_tks == null)
            {
                _tks = new Dictionary<object, Token>();
                AddToken('+', Tkid.ADD, Tktype.ADDSUB);
                AddToken('-', Tkid.SUB, Tktype.ADDSUB);
                AddToken('(', Tkid.OPEN, Tktype.GROUP);
                AddToken(')', Tkid.CLOSE, Tktype.GROUP);
                AddToken('*', Tkid.MUL, Tktype.MULDIV);
                AddToken('.', Tkid.PERIOD, Tktype.GROUP);
                AddToken('/', Tkid.DIV, Tktype.MULDIV);
                AddToken('\\', Tkid.DIVINT, Tktype.MULDIV);
                AddToken('=', Tkid.EQ, Tktype.COMPARE);
                AddToken('>', Tkid.GT, Tktype.COMPARE);
                AddToken('<', Tkid.LT, Tktype.COMPARE);
                AddToken('^', Tkid.POWER, Tktype.POWER);
                AddToken("<>", Tkid.NE, Tktype.COMPARE);
                AddToken(">=", Tkid.GE, Tktype.COMPARE);
                AddToken("<=", Tkid.LE, Tktype.COMPARE);

                // list separator is localized, not necessarily a comma
                // so it can't be on the static table
                //AddToken(',', TKID.COMMA, TKTYPE.GROUP);
            }
            return _tks;
        }
        void AddToken(object symbol, Tkid id, Tktype type)
        {
            var token = new Token(symbol, id, type);
            _tks.Add(symbol, token);
        }

        // build/get static keyword table
        Dictionary<string, FunctionDefinition> GetFunctionTable()
        {
            if (_fns == null)
            {
                // create table
                _fns = new Dictionary<string, FunctionDefinition>(StringComparer.InvariantCultureIgnoreCase);

                // register built-in functions (and constants)
                LogicalFunction.Register(this);
                MathFunction.Register(this);
                TextFunction.Register(this);
                StatisticalFunction.Register(this);
            }
            return _fns;
        }

        #endregion






        #region ** object model

        /// <summary>
        /// Parses a string into an <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">String to parse.</param>
        /// <returns>An <see cref="Expression"/> object that can be evaluated.</returns>
        public CalcExpression Parse(string expression)
        {
            // initialize
            _expr = expression;
            _len = _expr.Length;
            _ptr = 0;

            // skip leading equals sign
            if (_len > 0 && _expr[0] == '=')
            {
                _ptr++;
            }

            // parse the expression
            var expr = ParseExpression();

            // check for errors
            if (_token.Id != Tkid.END)
            {
                Throw();
            }

            // optimize expression
            if (OptimizeExpressions)
            {
                expr = expr.Optimize();
            }

            // done
            return expr;
        }

        /// <summary>
        /// Evaluates a string.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        /// <returns>The value of the expression.</returns>
        /// <remarks>
        /// If you are going to evaluate the same expression several times,
        /// it is more efficient to parse it only once using the <see cref="Parse"/>
        /// method and then using the Expression.Evaluate method to evaluate
        /// the parsed expression.
        /// </remarks>
		public object Evaluate(string expression)
        {
            var x = _cache != null
                ? _cache[expression]
                : Parse(expression);

            return x.Evaluate();
        }

        #endregion

        #region ** private stuff

        CalcExpression ParseExpression()
        {
            GetToken();
            return ParseCompare();
        }

        CalcExpression ParseCompare()
        {
            var x = ParseAddSub();
            while (_token.Type == Tktype.COMPARE)
            {
                var t = _token;
                GetToken();
                var exprArg = ParseAddSub();
                x = new CalcBinaryExpression(t, x, exprArg);
            }
            return x;
        }

        CalcExpression ParseAddSub()
        {
            var x = ParseMulDiv();
            while (_token.Type == Tktype.ADDSUB)
            {
                var t = _token;
                GetToken();
                var exprArg = ParseMulDiv();
                x = new CalcBinaryExpression(t, x, exprArg);
            }
            return x;
        }

        CalcExpression ParseMulDiv()
        {
            var x = ParsePower();
            while (_token.Type == Tktype.MULDIV)
            {
                var t = _token;
                GetToken();
                var a = ParsePower();
                x = new CalcBinaryExpression(t, x, a);
            }
            return x;
        }

        CalcExpression ParsePower()
        {
            var x = ParseUnary();
            while (_token.Type == Tktype.POWER)
            {
                var t = _token;
                GetToken();
                var a = ParseUnary();
                x = new CalcBinaryExpression(t, x, a);
            }
            return x;
        }

        CalcExpression ParseUnary()
        {
            // unary plus and minus
            if (_token.Id == Tkid.ADD || _token.Id == Tkid.SUB)
            {
                var t = _token;
                GetToken();
                var a = ParseAtom();
                return new CalcUnaryExpression(t, a);
            }

            // not unary, return atom
            return ParseAtom();
        }

        CalcExpression ParseAtom()
        {
            string id;
            CalcExpression x = null;
            FunctionDefinition fnDef = null;

            switch (_token.Type)
            {
                // literals
                case Tktype.LITERAL:
                    x = new CalcExpression(_token);
                    break;

                // identifiers
                case Tktype.IDENTIFIER:

                    // get identifier
                    id = (string)_token.Value;

                    // look for functions
                    if (_fns.TryGetValue(id, out fnDef))
                    {
                        var p = GetParameters();
                        var pCnt = p == null ? 0 : p.Count;
                        if (fnDef.ParmMin != -1 && pCnt < fnDef.ParmMin)
                        {
                            Throw("Too few parameters.");
                        }
                        if (fnDef.ParmMax != -1 && pCnt > fnDef.ParmMax)
                        {
                            Throw("Too many parameters.");
                        }
                        x = new CalcFunctionExpression(fnDef, p);
                    }

                    // look for simple variables (much faster than binding!)
                    if (Variables.ContainsKey(id))
                    {
                        x = new CalcVariableExpression(Variables, id);
                        break;
                    }

                    // look for external objects
                    var xObj = GetExternalObject(id);
                    if (xObj != null)
                    {
                        x = new CalcXObjectExpression(xObj);
                        break;
                    }

                    // look for bindings
                    if (DataContext != null)
                    {
                        var list = new List<BindingInfo>();
                        for (var t = _token; t != null; t = GetMember())
                        {
                            list.Add(new BindingInfo((string)t.Value, GetParameters()));
                        }
                        x = new CalcBindingExpression(this.DataContext, list, _ci);
                        break;
                    }
                    Throw("Unexpected identifier");
                    break;

                // sub-expressions
                case Tktype.GROUP:

                    // anything other than opening parenthesis is illegal here
                    if (_token.Id != Tkid.OPEN)
                    {
                        Throw("Expression expected.");
                    }

                    // get expression
                    GetToken();
                    x = ParseCompare();

                    // check that the parenthesis was closed
                    if (_token.Id != Tkid.CLOSE)
                    {
                        Throw("Unbalanced parenthesis.");
                    }

                    break;
            }

            // make sure we got something...
            if (x == null)
            {
                Throw();
            }

            // done
            GetToken();
            return x;
        }

        #endregion

        #region ** parser

        void GetToken()
        {
            // eat white space 
            while (_ptr < _len && _expr[_ptr] <= ' ')
            {
                _ptr++;
            }

            // are we done?
            if (_ptr >= _len)
            {
                _token = new Token(null, Tkid.END, Tktype.GROUP);
                return;
            }

            // prepare to parse
            int i;
            var c = _expr[_ptr];

            // operators
            // this gets called a lot, so it's pretty optimized.
            // note that operators must start with non-letter/digit characters.
            var isLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            var isDigit = c >= '0' && c <= '9';
            if (!isLetter && !isDigit)
            {
                // if this is a number starting with a decimal, don't parse as operator
                var nxt = _ptr + 1 < _len ? _expr[_ptr + 1] : 0;
                bool isNumber = c == _decimal && nxt >= '0' && nxt <= '9';
                if (!isNumber)
                {
                    // look up localized list separator
                    if (c == _listSep)
                    {
                        _token = new Token(c, Tkid.COMMA, Tktype.GROUP);
                        _ptr++;
                        return;
                    }

                    // look up single-char tokens on table
                    Token tk;
                    if (_tks.TryGetValue(c, out tk))
                    {
                        // save token we found
                        _token = tk;
                        _ptr++;

                        // look for double-char tokens (special case)
                        if (_ptr < _len && (c == '>' || c == '<'))
                        {
                            if (_tks.TryGetValue(_expr.Substring(_ptr - 1, 2), out tk))
                            {
                                _token = tk;
                                _ptr++;
                            }
                        }

                        // found token on the table
                        return;
                    }
                }
            }

            // parse numbers
            if (isDigit || c == _decimal)
            {
                var sci = false;
                var pct = false;
                var div = -1.0; // use double, not int (this may get really big)
                var val = 0.0;
                for (i = 0; i + _ptr < _len; i++)
                {
                    c = _expr[_ptr + i];

                    // digits always OK
                    if (c >= '0' && c <= '9')
                    {
                        val = val * 10 + (c - '0');
                        if (div > -1)
                        {
                            div *= 10;
                        }
                        continue;
                    }

                    // one decimal is OK
                    if (c == _decimal && div < 0)
                    {
                        div = 1;
                        continue;
                    }

                    // scientific notation?
                    if ((c == 'E' || c == 'e') && !sci)
                    {
                        sci = true;
                        c = _expr[_ptr + i + 1];
                        if (c == '+' || c == '-') i++;
                        continue;
                    }

                    // percentage?
                    if (c == _percent)
                    {
                        pct = true;
                        i++;
                        break;
                    }

                    // end of literal
                    break;
                }

                // end of number, get value
                if (!sci)
                {
                    // much faster than ParseDouble
                    if (div > 1)
                    {
                        val /= div;
                    }
                    if (pct)
                    {
                        val /= 100.0;
                    }
                }
                else
                {
                    var lit = _expr.Substring(_ptr, i);
                    val = ParseDouble(lit, _ci);
                }

                // build token
                _token = new Token(val, Tkid.ATOM, Tktype.LITERAL);

                // advance pointer and return
                _ptr += i;
                return;
            }

            // parse strings
            if (c == '\"')
            {
                // look for end quote, skip double quotes
                for (i = 1; i + _ptr < _len; i++)
                {
                    c = _expr[_ptr + i];
                    if (c != '\"') continue;
                    char cNext = i + _ptr < _len - 1 ? _expr[_ptr + i + 1] : ' ';
                    if (cNext != '\"') break;
                    i++;
                }

                // check that we got the end of the string
                if (c != '\"')
                {
                    Throw("Can't find final quote.");
                }

                // end of string
                var lit = _expr.Substring(_ptr + 1, i - 1);
                _ptr += i + 1;
                _token = new Token(lit.Replace("\"\"", "\""), Tkid.ATOM, Tktype.LITERAL);
                return;
            }

            // parse dates (review)
            if (c == '#')
            {
                // look for end #
                for (i = 1; i + _ptr < _len; i++)
                {
                    c = _expr[_ptr + i];
                    if (c == '#') break;
                }

                // check that we got the end of the date
                if (c != '#')
                {
                    Throw("Can't find final date delimiter ('#').");
                }

                // end of date
                var lit = _expr.Substring(_ptr + 1, i - 1);
                _ptr += i + 1;
                _token = new Token(DateTime.Parse(lit, _ci), Tkid.ATOM, Tktype.LITERAL);
                return;
            }

            // identifiers (functions, objects) must start with alpha or underscore
            if (!isLetter && c != '_' && (_idChars == null || _idChars.IndexOf(c) < 0))
            {
                Throw("Identifier expected.");
            }

            // and must contain only letters/digits/_idChars
            for (i = 1; i + _ptr < _len; i++)
            {
                c = _expr[_ptr + i];
                isLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
                isDigit = c >= '0' && c <= '9';
                if (!isLetter && !isDigit && c != '_' && (_idChars == null || _idChars.IndexOf(c) < 0))
                {
                    break;
                }
            }

            // got identifier
            var id = _expr.Substring(_ptr, i);
            _ptr += i;
            _token = new Token(id, Tkid.ATOM, Tktype.IDENTIFIER);
        }

        static double ParseDouble(string str, CultureInfo ci)
        {
            if (str.Length > 0 && str[str.Length - 1] == ci.NumberFormat.PercentSymbol[0])
            {
                str = str.Substring(0, str.Length - 1);
                return double.Parse(str, NumberStyles.Any, ci) / 100.0;
            }
            return double.Parse(str, NumberStyles.Any, ci);
        }

        List<CalcExpression> GetParameters() // e.g. myfun(a, b, c+2)
        {
            // check whether next token is a (, 
            // restore state and bail if it's not
            var pos = _ptr;
            var tk = _token;
            GetToken();
            if (_token.Id != Tkid.OPEN)
            {
                _ptr = pos;
                _token = tk;
                return null;
            }

            // check for empty Parameter list
            pos = _ptr;
            GetToken();
            if (_token.Id == Tkid.CLOSE)
            {
                return null;
            }
            _ptr = pos;

            // get Parameters until we reach the end of the list
            var parms = new List<CalcExpression>();
            var expr = ParseExpression();
            parms.Add(expr);
            while (_token.Id == Tkid.COMMA)
            {
                expr = ParseExpression();
                parms.Add(expr);
            }

            // make sure the list was closed correctly
            if (_token.Id != Tkid.CLOSE)
            {
                Throw();
            }

            // done
            return parms;
        }

        Token GetMember()
        {
            // check whether next token is a MEMBER token ('.'), 
            // restore state and bail if it's not
            var pos = _ptr;
            var tk = _token;
            GetToken();
            if (_token.Id != Tkid.PERIOD)
            {
                _ptr = pos;
                _token = tk;
                return null;
            }

            // skip member token
            GetToken();
            if (_token.Type != Tktype.IDENTIFIER)
            {
                Throw("Identifier expected");
            }
            return _token;
        }

        #endregion

        #region ** static helpers

        static void Throw()
        {
            Throw("Syntax error.");
        }

        static void Throw(string msg)
        {
            throw new Exception(msg);
        }

        #endregion
    }
}