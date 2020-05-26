using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    class CalcFunctionExpression : CalcExpression
    {
        // ** fields
        FunctionDefinition _fn;
        List<CalcExpression> _parms;

        // ** ctor
        internal CalcFunctionExpression()
        {
        }

        public CalcFunctionExpression(FunctionDefinition function, List<CalcExpression> parms)
        {
            _fn = function;
            _parms = parms;
        }

        // ** object model
        override public object Evaluate()
        {
            return _fn.Function(_parms);
        }

        public override CalcExpression Optimize()
        {
            bool allLits = true;
            if (_parms != null)
            {
                for (int i = 0; i < _parms.Count; i++)
                {
                    var p = _parms[i].Optimize();
                    _parms[i] = p;
                    if (p._token.Type != TKTYPE.LITERAL)
                    {
                        allLits = false;
                    }
                }
            }
            return allLits
                ? new CalcExpression(this.Evaluate())
                : this;
        }
    }
}
