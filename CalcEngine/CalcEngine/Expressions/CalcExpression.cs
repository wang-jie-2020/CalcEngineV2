using System;
using System.Globalization;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// Base class that represents parsed expressions.
    /// </summary>
    /// <remarks>
    /// For example:
    /// <code>
    /// Expression expr = scriptEngine.Parse(strExpression);
    /// object val = expr.Evaluate();
    /// </code>
    /// </remarks>
    public class CalcExpression : IComparable<CalcExpression>
    {

        //---------------------------------------------------------------------------
        #region ** fields

        internal Token _token;
        static CultureInfo _ci = CultureInfo.InvariantCulture;

        #endregion

        //---------------------------------------------------------------------------
        #region ** ctors

        internal CalcExpression()
        {
            _token = new Token(null, TKID.ATOM, TKTYPE.IDENTIFIER);
        }

        internal CalcExpression(object value)
        {
            _token = new Token(value, TKID.ATOM, TKTYPE.LITERAL);
        }

        internal CalcExpression(Token tk)
        {
            _token = tk;
        }

        #endregion

        //---------------------------------------------------------------------------
        #region ** object model

        public virtual object Evaluate()
        {
            if (_token.Type != TKTYPE.LITERAL)
            {
                throw new ArgumentException("Bad expression.");
            }
            return _token.Value;
        }

        public virtual CalcExpression Optimize()
        {
            return this;
        }

        #endregion

        //---------------------------------------------------------------------------
        #region ** implicit converters

        public static implicit operator string(CalcExpression x)
        {
            var v = x.Evaluate();
            return v == null ? string.Empty : v.ToString();
        }

        public static implicit operator double(CalcExpression x)
        {
            // evaluate
            var v = x.Evaluate();

            // handle doubles
            if (v is double)
            {
                return (double)v;
            }

            // handle booleans
            if (v is bool)
            {
                return (bool)v ? 1 : 0;
            }

            // handle dates
            if (v is DateTime)
            {
                return ((DateTime)v).ToOADate();
            }

            // handle nulls
            if (v == null)
            {
                return 0;
            }

            // handle everything else
            return (double)Convert.ChangeType(v, typeof(double), _ci);
        }

        public static implicit operator bool(CalcExpression x)
        {
            // evaluate
            var v = x.Evaluate();

            // handle booleans
            if (v is bool)
            {
                return (bool)v;
            }

            // handle nulls
            if (v == null)
            {
                return false;
            }

            // handle doubles
            if (v is double)
            {
                return (double)v == 0 ? false : true;
            }

            // handle everything else
            return (double)x == 0 ? false : true;
        }

        public static implicit operator DateTime(CalcExpression x)
        {
            // evaluate
            var v = x.Evaluate();

            // handle dates
            if (v is DateTime)
            {
                return (DateTime)v;
            }

            // handle doubles
            if (v is double)
            {
                return DateTime.FromOADate((double)x);
            }

            // handle everything else
            return (DateTime)Convert.ChangeType(v, typeof(DateTime), _ci);
        }

        #endregion

        //---------------------------------------------------------------------------
        #region ** IComparable<CalcExpression>

        public int CompareTo(CalcExpression other)
        {
            // get both values
            var c1 = this.Evaluate() as IComparable;
            var c2 = other.Evaluate() as IComparable;

            // handle nulls
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

            // make sure types are the same
            if (c1.GetType() != c2.GetType())
            {
                c2 = Convert.ChangeType(c2, c1.GetType()) as IComparable;
            }

            // compare
            return c1.CompareTo(c2);
        }

        #endregion

    }
}
