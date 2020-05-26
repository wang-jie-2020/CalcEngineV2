using System;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// Unary expression, e.g. +123
    /// </summary>
    class CalcUnaryExpression : CalcExpression
    {
        // ** fields
        CalcExpression _expr;

        // ** ctor
        public CalcUnaryExpression(Token tk, CalcExpression expr) : base(tk)
        {
            _expr = expr;
        }

        // ** object model
        override public object Evaluate()
        {
            switch (_token.ID)
            {
                case TKID.ADD:
                    return +(double)_expr;
                case TKID.SUB:
                    return -(double)_expr;
            }
            throw new ArgumentException("Bad expression.");
        }

        public override CalcExpression Optimize()
        {
            _expr = _expr.Optimize();
            return _expr._token.Type == TKTYPE.LITERAL
                ? new CalcExpression(this.Evaluate())
                : this;
        }
    }
}
