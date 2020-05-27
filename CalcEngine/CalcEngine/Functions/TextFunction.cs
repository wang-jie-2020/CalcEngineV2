using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CalcEngine.Expressions;

namespace CalcEngine.Functions
{
    internal static class TextFunction
    {
        public static void Register(CalcEngine ce)
        {
            ce.RegisterFunction("CHAR", 1, _Char);
            ce.RegisterFunction("CODE", 1, Code);
            ce.RegisterFunction("CONCAT", 1, int.MaxValue, Concat);

            ce.RegisterFunction("FIND", 2, 3, Find);    //FIND(input,para,startIndex=0)

            ce.RegisterFunction("LEFT", 1, 2, Left);    //LEFT(input,length=1)
            ce.RegisterFunction("LEN", 1, Len);
            ce.RegisterFunction("LOWER", 1, Lower);

            ce.RegisterFunction("MID", 3, Mid); //MID(input,startIndex,endIndex)

            ce.RegisterFunction("REPLACE", 4, Replace); //REPLACE(input,startIndex,length,replace)
            ce.RegisterFunction("REPT", 2, Rept);   //Rept(input,repeatCount)
            ce.RegisterFunction("RIGHT", 1, 2, Right); //RIGHT(input,length=1)

            ce.RegisterFunction("SEARCH", 2, Search);
            //ce.RegisterFunction("SUBSTITUTE", 3, 4, Substitute);

            ce.RegisterFunction("TOSTRING", 1, ToString);
            ce.RegisterFunction("TEXT", 2, _Text);  //TEXT(input,format)
            ce.RegisterFunction("TRIM", 1, Trim);
            ce.RegisterFunction("UPPER", 1, Upper);
            ce.RegisterFunction("VALUE", 1, Value);
        }

        static object _Char(List<CalcExpression> p)
        {
            var c = (char)(int)p[0];
            return c.ToString();
        }

        static object Code(List<CalcExpression> p)
        {
            var s = (string)p[0];
            return (int)s[0];
        }

        static object Concat(List<CalcExpression> p)
        {
            var sb = new StringBuilder();
            foreach (var x in p)
            {
                sb.Append((string)x);
            }
            return sb.ToString();
        }

        static object Find(List<CalcExpression> p)
        {
            return IndexOf(p, StringComparison.Ordinal);
        }

        static int IndexOf(List<CalcExpression> p, StringComparison cmp)
        {
            var srch = (string)p[0];
            var text = (string)p[1];
            var start = 0;
            if (p.Count > 2)
            {
                start = (int)p[2] - 1;
            }
            var index = text.IndexOf(srch, start, cmp);
            return index > -1 ? index + 1 : index;
        }

        static object Left(List<CalcExpression> p)
        {
            var n = 1;
            if (p.Count > 1)
            {
                n = (int)p[1];
            }
            return ((string)p[0]).Substring(0, n);
        }

        static object Len(List<CalcExpression> p)
        {
            return ((string)p[0]).Length;
        }

        static object Lower(List<CalcExpression> p)
        {
            return ((string)p[0]).ToLower();
        }

        static object Mid(List<CalcExpression> p)
        {
            return ((string)p[0]).Substring((int)p[1] - 1, (int)p[2]);
        }

        static object Replace(List<CalcExpression> p)
        {
            var s = (string)p[0];
            var start = (int)p[1] - 1;
            var len = (int)p[2];
            var rep = (string)p[3];

            var sb = new StringBuilder();
            sb.Append(s.Substring(0, start));
            sb.Append(rep);
            sb.Append(s.Substring(start + len));

            return sb.ToString();
        }

        static object Rept(List<CalcExpression> p)
        {
            var sb = new StringBuilder();
            var s = (string)p[0];
            for (int i = 0; i < (int)p[1]; i++)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        static object Right(List<CalcExpression> p)
        {
            var n = 1;
            if (p.Count > 1)
            {
                n = (int)p[1];
            }
            var s = (string)p[0];
            return s.Substring(s.Length - n);
        }

        static object Search(List<CalcExpression> p)
        {
            return IndexOf(p, StringComparison.OrdinalIgnoreCase);
        }

        //static object Substitute(List<CalcExpression> p)
        //{
        //    // get parameters
        //    var text = (string)p[0];
        //    var oldText = (string)p[1];
        //    var newText = (string)p[2];

        //    // if index not supplied, replace all
        //    if (p.Count == 3)
        //    {
        //        return text.Replace(oldText, newText);
        //    }

        //    // replace specific instance
        //    int index = (int)p[3];
        //    if (index < 1)
        //    {
        //        throw new Exception("Invalid index in Substitute.");
        //    }
        //    int pos = text.IndexOf(oldText);
        //    while (pos > -1 && index > 1)
        //    {
        //        pos = text.IndexOf(oldText, pos + 1);
        //        index--;
        //    }
        //    return pos > -1
        //        ? text.Substring(0, pos) + newText + text.Substring(pos + oldText.Length)
        //        : text;
        //}

        static object ToString(List<CalcExpression> p)
        {
            return (string)p[0];
        }

        static object _Text(List<CalcExpression> p)
        {
            return ((double)p[0]).ToString((string)p[1], CultureInfo.CurrentCulture);
        }

        static object Trim(List<CalcExpression> p)
        {
            return ((string)p[0]).Trim();
        }

        static object Upper(List<CalcExpression> p)
        {
            return ((string)p[0]).ToUpper();
        }

        static object Value(List<CalcExpression> p)
        {
            return double.Parse((string)p[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
