using System;
using System.Globalization;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 表达式基类
    /// </summary>
    public class CalcExpression : IComparable<CalcExpression>
    {
        internal Token _token;
        static CultureInfo _ci = CultureInfo.InvariantCulture;

        #region 构造函数

        internal CalcExpression()
        {
            _token = new Token(null, Tkid.ATOM, Tktype.LITERAL);
        }

        internal CalcExpression(object value)
        {
            _token = new Token(value, Tkid.ATOM, Tktype.LITERAL);
        }

        internal CalcExpression(Token tk)
        {
            _token = tk;
        }

        #endregion

        #region 核心方法

        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <returns></returns>
        public virtual object Evaluate()
        {
            //除了字面量，都需要重写此方法
            if (_token.Type != Tktype.LITERAL)
            {
                throw new ArgumentException("Bad expression.");
            }
            return _token.Value;
        }

        /// <summary>
        /// 表达式优化，替换已计算结果
        /// </summary>
        /// <returns></returns>
        public virtual CalcExpression Optimize()
        {
            return this;
        }

        #endregion

        #region 重载符号

        public static implicit operator string(CalcExpression x)
        {
            var v = x.Evaluate();
            return v == null ? string.Empty : v.ToString();
        }

        public static implicit operator double(CalcExpression x)
        {
            var v = x.Evaluate();

            if (v is double)
            {
                return (double)v;
            }

            if (v is bool)
            {
                return (bool)v ? 1 : 0;
            }

            if (v is DateTime)
            {
                return ((DateTime)v).ToOADate();
            }

            if (v == null)
            {
                return 0;
            }

            return (double)Convert.ChangeType(v, typeof(double), _ci);
        }

        public static implicit operator bool(CalcExpression x)
        {
            var v = x.Evaluate();

            if (v is bool)
            {
                return (bool)v;
            }

            if (v == null)
            {
                return false;
            }

            if (v is double)
            {
                return (double)v == 0 ? false : true;
            }

            return (double)x == 0 ? false : true;
        }

        public static implicit operator DateTime(CalcExpression x)
        {
            var v = x.Evaluate();

            if (v is DateTime)
            {
                return (DateTime)v;
            }

            if (v is double)
            {
                return DateTime.FromOADate((double)x);
            }

            return (DateTime)Convert.ChangeType(v, typeof(DateTime), _ci);
        }

        #endregion

        #region IComparable<CalcExpression>

        public int CompareTo(CalcExpression other)
        {
            var c1 = this.Evaluate() as IComparable;
            var c2 = other.Evaluate() as IComparable;

            if (c1 == null && c2 == null)
            {
                return 0;
            }
            if (c2 == null)
            {
                return -1;
            }
            if (c1 == null)
            {
                return +1;
            }

            if (c1.GetType() != c2.GetType())
            {
                c2 = Convert.ChangeType(c2, c1.GetType()) as IComparable;
            }

            return c1.CompareTo(c2);
        }

        #endregion

    }
}
