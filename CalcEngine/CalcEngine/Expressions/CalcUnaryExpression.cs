using System;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 一元表达式
    /// </summary>
    internal class CalcUnaryExpression : CalcExpression
    {
        CalcExpression _expr;

        public CalcUnaryExpression(Token tk, CalcExpression expr) : base(tk)
        {
            _expr = expr;
        }

        public override object Evaluate()
        {
            switch (_token.Id)
            {
                case Tkid.ADD:
                    return +(double)_expr;
                case Tkid.SUB:
                    return -(double)_expr;
            }
            throw new ArgumentException("Bad expression.");
        }

        public override CalcExpression Optimize()
        {
            _expr = _expr.Optimize();
            return _expr._token.Type == Tktype.LITERAL
                ? new CalcExpression(this.Evaluate())
                : this;
        }
    }
}
