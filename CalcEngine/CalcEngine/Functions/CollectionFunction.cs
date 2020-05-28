using System;
using System.Collections.Generic;
using System.Text;
using CalcEngine.Expressions;

namespace CalcEngine.Functions
{
    public class CollectionFunction
    {
        public static void Register(CalcEngine ce)
        {

        }

        static object And(List<CalcExpression> p)
        {
            var b = true;
            foreach (var v in p)
            {
                b = b && (bool)v;
            }
            return b;
        }
    }
}
