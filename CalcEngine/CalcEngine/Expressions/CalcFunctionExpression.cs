using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 类似委托表达式，计算函数结果
    /// </summary>
    internal class CalcFunctionExpression : CalcExpression
    {
        FunctionDefinition _fn;
        List<CalcExpression> _parms;

        public CalcFunctionExpression(FunctionDefinition function, List<CalcExpression> parms)
        {
            _fn = function;
            _parms = parms;
        }

        public override object Evaluate()
        {
            return _fn.Function(_parms);
        }

        //王杰：不进行字面量优化可以扩展功能，字面量优化只能针对当前数据源
        //public override CalcExpression Optimize()
        //{
        //    bool allLits = true;
        //    if (_parms != null)
        //    {
        //        for (int i = 0; i < _parms.Count; i++)
        //        {
        //            var p = _parms[i].Optimize();
        //            _parms[i] = p;
        //            if (p._token.Type != Tktype.LITERAL)
        //            {
        //                allLits = false;
        //            }
        //        }
        //    }
        //    return allLits
        //        ? new CalcExpression(this.Evaluate())
        //        : this;
        //}
    }
}
