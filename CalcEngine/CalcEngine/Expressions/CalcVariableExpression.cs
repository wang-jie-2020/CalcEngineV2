using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    class CalcVariableExpression : CalcExpression
    {
        IDictionary<string, object> _dct;
        string _name;

        public CalcVariableExpression(IDictionary<string, object> dct, string name)
        {
            _dct = dct;
            _name = name;
        }

        public override object Evaluate()
        {
            return _dct[_name];
        }
    }
}
