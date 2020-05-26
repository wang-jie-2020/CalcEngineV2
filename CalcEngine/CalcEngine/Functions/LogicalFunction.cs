using System;
using System.Collections.Generic;
using System.Text;
using CalcEngine.Expressions;

namespace CalcEngine.Functions
{
    public static class LogicalFunction
    {
        public static void Register(CalcEngine ce)
        {
            ce.RegisterFunction("AND", 1, int.MaxValue, And);
            ce.RegisterFunction("OR", 1, int.MaxValue, Or);
            ce.RegisterFunction("NOT", 1, Not);
            ce.RegisterFunction("IF", 3, If);
            ce.RegisterFunction("TRUE", 0, True);
            ce.RegisterFunction("FALSE", 0, False);
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

        static object Or(List<CalcExpression> p)
        {
            var b = false;
            foreach (var v in p)
            {
                b = b || (bool)v;
            }
            return b;
        }

        static object Not(List<CalcExpression> p)
        {
            return !(bool)p[0];
        }

        static object If(List<CalcExpression> p)
        {
            return (bool)p[0]
                ? p[1].Evaluate()
                : p[2].Evaluate();
        }

        static object True(List<CalcExpression> p)
        {
            return true;
        }

        static object False(List<CalcExpression> p)
        {
            return false;
        }
    }
}
