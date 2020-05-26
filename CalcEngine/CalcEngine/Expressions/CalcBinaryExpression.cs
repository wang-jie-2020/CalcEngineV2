using System;

namespace CalcEngine.Expressions
{
    class CalcBinaryExpression : CalcExpression
    {
        // ** fields
        CalcExpression _lft;
        CalcExpression _rgt;

        // ** ctor
        public CalcBinaryExpression(Token tk, CalcExpression exprLeft, CalcExpression exprRight) : base(tk)
        {
            _lft = exprLeft;
            _rgt = exprRight;
        }

        // ** object model
        override public object Evaluate()
        {
            // handle comparisons
            if (_token.Type == TKTYPE.COMPARE)
            {
                var cmp = _lft.CompareTo(_rgt);
                switch (_token.ID)
                {
                    case TKID.GT: return cmp > 0;
                    case TKID.LT: return cmp < 0;
                    case TKID.GE: return cmp >= 0;
                    case TKID.LE: return cmp <= 0;
                    case TKID.EQ: return cmp == 0;
                    case TKID.NE: return cmp != 0;
                }
            }

            // handle everything else
            switch (_token.ID)
            {
                case TKID.ADD:
                    return (double)_lft + (double)_rgt;
                case TKID.SUB:
                    return (double)_lft - (double)_rgt;
                case TKID.MUL:
                    return (double)_lft * (double)_rgt;
                case TKID.DIV:
                    return (double)_lft / (double)_rgt;
                case TKID.DIVINT:
                    return (double)(int)((double)_lft / (double)_rgt);
                case TKID.MOD:
                    return (double)(int)((double)_lft % (double)_rgt);
                case TKID.POWER:
                    var a = (double)_lft;
                    var b = (double)_rgt;
                    if (b == 0.0) return 1.0;
                    if (b == 0.5) return Math.Sqrt(a);
                    if (b == 1.0) return a;
                    if (b == 2.0) return a * a;
                    if (b == 3.0) return a * a * a;
                    if (b == 4.0) return a * a * a * a;
                    return Math.Pow((double)_lft, (double)_rgt);
            }
            throw new ArgumentException("Bad expression.");
        }
        public override CalcExpression Optimize()
        {
            _lft = _lft.Optimize();
            _rgt = _rgt.Optimize();
            return _lft._token.Type == TKTYPE.LITERAL && _rgt._token.Type == TKTYPE.LITERAL
                ? new CalcExpression(this.Evaluate())
                : this;
        }
    }
}
