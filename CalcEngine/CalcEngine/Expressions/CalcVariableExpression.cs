using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 字典键值
    /// </summary>
    internal class CalcVariableExpression : CalcExpression
    {
        IDictionary<string, object> _dict;
        string _key;

        public CalcVariableExpression(IDictionary<string, object> dict, string key)
        {
            _dict = dict;
            _key = key;
        }

        public override object Evaluate()
        {
            return _dict[_key];
        }
    }
}
