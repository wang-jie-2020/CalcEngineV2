using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Mime;
using CalcEngine.Expressions;
using CalcEngine.Functions;

namespace CalcEngine
{
    /// <summary>
    /// 计算引擎核心
    /// </summary>
    public partial class CalcEngine
    {
        #region 设置、持久字段

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

        #endregion

        #region Init

        void InitSymbol()
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
                //AddToken(',', Tkid.COMMA, Tktype.GROUP);       // list separator is localized, not necessarily a comma,so it can't be on the static table
            }
        }

        void AddToken(object symbol, Tkid id, Tktype type)
        {
            var token = new Token(symbol, id, type);
            _tks.Add(symbol, token);
        }

        void InitFunction()
        {
            if (_fns == null)
            {
                _fns = new Dictionary<string, FunctionDefinition>(StringComparer.InvariantCultureIgnoreCase);

                LogicalFunction.Register(this);
                MathFunction.Register(this);
                TextFunction.Register(this);
                StatisticalFunction.Register(this);
            }
        }

        #endregion

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public object Evaluate(string expression)
        {
            //默认将表达式加入缓存，提高效率
            var x = _cache != null
                ? _cache[expression]
                : Parse(expression);

            return x.Evaluate();
        }

        /// <summary>
        /// 字符串解析为表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public CalcExpression Parse(string expression)
        {
            _expr = expression;
            _len = _expr.Length;
            _ptr = 0;

            //第一个‘=’字符不作为解析内容
            if (_len > 0 && _expr[0] == '=')
            {
                _ptr++;
            }

            //递归
            var expr = ParseExpression();

            if (_token.Id != Tkid.END)
            {
                Throw();
            }

            if (OptimizeExpressions)
            {
                expr = expr.Optimize();
            }

            return expr;
        }

        #region 核心函数

        string _expr;    //表达式                  
        int _len;        //表达式长度                
        int _ptr;        //当前指向 
        Token _token;    //当前Token               

        #region ** private stuff

        /// <summary>
        /// 过程开始
        /// </summary>
        /// <returns
        /// ></returns>
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
            //空格不计
            while (_ptr < _len && _expr[_ptr] <= ' ')
            {
                _ptr++;
            }

            //到达字符串尾，结束
            if (_ptr >= _len)
            {
                _token = new Token(null, Tkid.END, Tktype.GROUP);
                return;
            }

            int index = 0;  //计数提出来，少写几个int i

            var c = _expr[_ptr];

            //转义字符
            if (c == '\"')
            {
                index = 1;
                for (; index + _ptr < _len; index++)
                {
                    c = _expr[_ptr + index];
                    if (c != '\"')
                        continue;

                    //跳过双重转移，即"\\xxxx"
                    char cNext = index + _ptr < _len - 1 ? _expr[_ptr + index + 1] : ' ';
                    if (cNext != '\"')
                        break;

                    index++;
                }

                if (c != '\"')
                {
                    Throw("Can't find final quote.");
                }

                var lit = _expr.Substring(_ptr + 1, index - 1);
                _ptr += index + 1;
                _token = new Token(lit.Replace("\"\"", "\""), Tkid.ATOM, Tktype.LITERAL);
                return;
            }

            // #代表什么？？不清楚。。。
            if (c == '#')
            {
                index = 1;
                for (; index + _ptr < _len; index++)
                {
                    c = _expr[_ptr + index];
                    if (c == '#')
                        break;
                }

                if (c != '#')
                {
                    Throw("Can't find final date delimiter ('#').");
                }

                var lit = _expr.Substring(_ptr + 1, index - 1);
                _ptr += index + 1;
                _token = new Token(DateTime.Parse(lit, _ci), Tkid.ATOM, Tktype.LITERAL);
                return;
            }

            var isLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
            var isDigit = c >= '0' && c <= '9';

            /*  
             *  非字符、数字的特殊情况：
             *  1.浮点数的开始，比如'.5'，这种情况看后一位是不是数字
             *  2.操作符，这种情况从符号集中查找
             *  其余情况不考虑继续处理，只作为原子性Node扔出去
             */
            if (!isLetter && !isDigit)
            {
                var nxt = _ptr + 1 < _len ? _expr[_ptr + 1] : 0;
                bool isNumber = c == _decimal && nxt >= '0' && nxt <= '9';
                if (!isNumber)
                {
                    /*
                     *  不理解这个if的含义：
                     *  _listSep表示本地化下的数字步长分割标志，比如中文下1,000,000的','
                     *  存在单独出现情况？
                     */
                    if (c == _listSep)
                    {
                        _token = new Token(c, Tkid.COMMA, Tktype.GROUP);
                        _ptr++;
                        return;
                    }

                    //从字符集中尝试获取，比如+-*/
                    Token tk;
                    if (_tks.TryGetValue(c, out tk))
                    {
                        _token = tk;
                        _ptr++;

                        //可能存在的>=或<=请款情况，单独取一个字符不足以表示符号
                        if (_ptr < _len && (c == '>' || c == '<'))
                        {
                            if (_tks.TryGetValue(_expr.Substring(_ptr - 1, 2), out tk))
                            {
                                _token = tk;
                                _ptr++;
                            }
                        }

                        return;
                    }
                }
            }

            /*
             *  数字，考虑两个情况：1.科学计数法  2.百分数
             *  总是作为字面量返回 
             */
            if (isDigit || c == _decimal)
            {
                var sci = false;    //科学计数
                var pct = false;    //百度比

                var div = -1.0;
                var val = 0.0;

                index = 0;
                for (; index + _ptr < _len; index++)
                {
                    c = _expr[_ptr + index];

                    if (c >= '0' && c <= '9')
                    {
                        val = val * 10 + (c - '0');
                        if (div > -1)
                        {
                            div *= 10;
                        }
                        continue;
                    }

                    if (c == _decimal && div < 0)
                    {
                        div = 1;
                        continue;
                    }

                    if ((c == 'E' || c == 'e') && !sci)
                    {
                        sci = true;
                        c = _expr[_ptr + index + 1];
                        if (c == '+' || c == '-') index++;
                        continue;
                    }

                    if (c == _percent)
                    {
                        pct = true;
                        index++;
                        break;
                    }

                    break;
                }

                if (!sci)
                {
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
                    var lit = _expr.Substring(_ptr, index);
                    val = ParseDouble(lit, _ci);
                }

                _token = new Token(val, Tkid.ATOM, Tktype.LITERAL);
                _ptr += index;
                return;
            }

            //非字母、'_'，也不包含在自定义的前缀的字符，属于非期望
            if (!isLetter && c != '_' && (_idChars == null || _idChars.IndexOf(c) < 0))
            {
                Throw("Identifier expected.");
            }

            /*
             *  字符情况，出现字符就认为是内置功能函数的名称
             *  约定情况下，函数总是以字母、或下划线起始的
             */
            index = 1;
            for (; index + _ptr < _len; index++)
            {
                c = _expr[_ptr + index];
                isLetter = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
                isDigit = c >= '0' && c <= '9';
                if (!isLetter && !isDigit && c != '_' && (_idChars == null || _idChars.IndexOf(c) < 0))
                {
                    break;
                }
            }

            var id = _expr.Substring(_ptr, index);
            _ptr += index;
            _token = new Token(id, Tkid.ATOM, Tktype.IDENTIFIER);
        }

        double ParseDouble(string str, CultureInfo ci)
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

        #endregion

        #region Throw Error

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
