using System;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 二元表达式
    /// </summary>
    internal class CalcBinaryExpression : CalcExpression
    {
        CalcExpression _lft;
        CalcExpression _rgt;

        public CalcBinaryExpression(Token tk, CalcExpression exprLeft, CalcExpression exprRight) : base(tk)
        {
            _lft = exprLeft;
            _rgt = exprRight;
        }

        public override object Evaluate()
        {
            if (_token.Type == Tktype.COMPARE)
            {
                var cmp = _lft.CompareTo(_rgt);
                switch (_token.Id)
                {
                    case Tkid.GT:
                        return cmp > 0;
                    case Tkid.LT:
                        return cmp < 0;
                    case Tkid.GE:
                        return cmp >= 0;
                    case Tkid.LE:
                        return cmp <= 0;
                    case Tkid.EQ:
                        return cmp == 0;
                    case Tkid.NE:
                        return cmp != 0;
                }
            }

            switch (_token.Id)
            {
                case Tkid.ADD:
                    return (double)_lft + (double)_rgt;
                case Tkid.SUB:
                    return (double)_lft - (double)_rgt;
                case Tkid.MUL:
                    return (double)_lft * (double)_rgt;
                case Tkid.DIV:
                    return (double)_lft / (double)_rgt;
                case Tkid.DIVINT:
                    return (double)(int)((double)_lft / (double)_rgt);
                case Tkid.MOD:
                    return (double)(int)((double)_lft % (double)_rgt);
                case Tkid.POWER:
                    return Math.Pow((double)_lft, (double)_rgt);
            }

            throw new ArgumentException("Bad expression.");
        }

        public override CalcExpression Optimize()
        {
            _lft = _lft.Optimize();
            _rgt = _rgt.Optimize();

            return _lft._token.Type == Tktype.LITERAL && _rgt._token.Type == Tktype.LITERAL
                ? new CalcExpression(this.Evaluate())
                : this;
        }
    }
}
