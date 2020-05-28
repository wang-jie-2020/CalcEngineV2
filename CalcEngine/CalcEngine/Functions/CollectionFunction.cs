using System.Collections;
using System.Collections.Generic;
using CalcEngine.Expressions;

namespace CalcEngine.Functions
{
    /// <summary>
    /// 王杰2020-5-28 新增
    /// 集合的功能
    /// </summary>
    internal class CollectionFunction
    {
        CalcEngine calcEngine;

        public CollectionFunction(CalcEngine ce)
        {
            calcEngine = ce;
        }

        public void Register()
        {
            calcEngine.RegisterFunction("WHERE", 2, Where);
            calcEngine.RegisterFunction("SELECT", 2, Select);
        }

        object Where(List<CalcExpression> p)
        {
            object dataContext = calcEngine.DataContext;

            try
            {
                List<object> result = new List<object>();

                IEnumerable range = p[0].Evaluate() as IEnumerable;
                foreach (var item in range)
                {
                    calcEngine.DataContext = item;

                    if ((bool)p[1])
                        result.Add(item);
                }

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                calcEngine.DataContext = dataContext;
            }
        }

        object Select(List<CalcExpression> p)
        {
            object dataContext = calcEngine.DataContext;

            try
            {
                List<object> result = new List<object>();

                IEnumerable range = p[0].Evaluate() as IEnumerable;
                foreach (var item in range)
                {
                    calcEngine.DataContext = item;

                    var r = p[1].Evaluate();
                    result.Add(r);
                }

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                calcEngine.DataContext = dataContext;
            }
        }
    }
}
