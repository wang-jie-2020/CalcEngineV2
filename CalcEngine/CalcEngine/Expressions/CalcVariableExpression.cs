using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 字典键值
    /// </summary>
    internal class CalcVariableExpression : CalcExpression
    {
        private readonly innerDataSource _innerData;
        private readonly string _key;

        public CalcVariableExpression(innerDataSource innerData, string key)
        {
            _innerData = innerData;
            _key = key;
        }

        public override object Evaluate()
        {
            return _innerData.Variables[_key];
        }
    }
}
