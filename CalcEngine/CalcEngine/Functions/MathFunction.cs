using CalcEngine.Expressions;
using CalcEngine.Functions.InternalFunction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CalcEngine.Functions
{
    /// <summary>
    /// 数学功能
    /// </summary>
    internal static class MathFunction
    {
        public static void Register(CalcEngine ce)
        {
            ce.RegisterFunction("ABS", 1, Abs);
            ce.RegisterFunction("ACOS", 1, Acos);
            ce.RegisterFunction("ASIN", 1, Asin);
            ce.RegisterFunction("ATAN", 1, Atan);
            ce.RegisterFunction("ATAN2", 2, Atan2);

            ce.RegisterFunction("CEILING", 1, Ceiling);
            ce.RegisterFunction("COS", 1, Cos);
            ce.RegisterFunction("COSH", 1, Cosh);

            ce.RegisterFunction("EXP", 1, Exp);
            ce.RegisterFunction("FLOOR", 1, Floor);

            ce.RegisterFunction("INT", 1, Int);

            ce.RegisterFunction("LN", 1, Ln);
            ce.RegisterFunction("LOG", 1, 2, Log);
            ce.RegisterFunction("LOG10", 1, Log10);

            ce.RegisterFunction("PI", 0, Pi);
            ce.RegisterFunction("POWER", 2, Power);

            ce.RegisterFunction("RAND", 0, Rand);
            ce.RegisterFunction("RANDBETWEEN", 2, RandBetween);
            ce.RegisterFunction("ROUND", 1, 2, Round);
            ce.RegisterFunction("ROUNDE", 1, 2, RoundE);

            ce.RegisterFunction("SIGN", 1, Sign);
            ce.RegisterFunction("SIN", 1, Sin);
            ce.RegisterFunction("SINH", 1, Sinh);
            ce.RegisterFunction("SQRT", 1, Sqrt);

            ce.RegisterFunction("TAN", 1, Tan);
            ce.RegisterFunction("TANH", 1, Tanh);
            ce.RegisterFunction("TRUNC", 1, Trunc);

            ce.RegisterFunction("WEIGHTED", 2, Weighted);
        }

        static object Abs(List<CalcExpression> p)
        {
            return Math.Abs((double)p[0]);
        }

        static object Acos(List<CalcExpression> p)
        {
            return Math.Acos((double)p[0]);
        }

        static object Asin(List<CalcExpression> p)
        {
            return Math.Asin((double)p[0]);
        }

        static object Atan(List<CalcExpression> p)
        {
            return Math.Atan((double)p[0]);
        }

        static object Atan2(List<CalcExpression> p)
        {
            return Math.Atan2((double)p[0], (double)p[1]);
        }

        static object Ceiling(List<CalcExpression> p)
        {
            return Math.Ceiling((double)p[0]);
        }

        static object Cos(List<CalcExpression> p)
        {
            return Math.Cos((double)p[0]);
        }

        static object Cosh(List<CalcExpression> p)
        {
            return Math.Cosh((double)p[0]);
        }

        static object Exp(List<CalcExpression> p)
        {
            return Math.Exp((double)p[0]);
        }

        static object Floor(List<CalcExpression> p)
        {
            return Math.Floor((double)p[0]);
        }

        static object Int(List<CalcExpression> p)
        {
            return Math.Truncate((double)p[0]);
        }

        static object Ln(List<CalcExpression> p)
        {
            return Math.Log((double)p[0]);
        }

        static object Log(List<CalcExpression> p)
        {
            var lbase = p.Count > 1 ? (double)p[1] : 10;
            return Math.Log((double)p[0], lbase);
        }

        static object Log10(List<CalcExpression> p)
        {
            return Math.Log10((double)p[0]);
        }

        static object Pi(List<CalcExpression> p)
        {
            return Math.PI;
        }

        static object Power(List<CalcExpression> p)
        {
            return Math.Pow((double)p[0], (double)p[1]);
        }

        static Random _rnd = new Random();
        static object Rand(List<CalcExpression> p)
        {
            return _rnd.NextDouble();
        }

        static object RandBetween(List<CalcExpression> p)
        {
            return _rnd.Next((int)(double)p[0], (int)(double)p[1]);
        }

        static object Round(List<CalcExpression> p)
        {
            var toDecimals = 0;
            if (p.Count > 1)
            {
                toDecimals = (int)p[p.Count - 1];
            }
            return Math.Round(p[0], toDecimals);
        }

        static object RoundE(List<CalcExpression> p)
        {
            var toDecimals = 0;
            if (p.Count > 1)
            {
                toDecimals = (int)p[p.Count - 1];
            }
            return Math.Round(p[0], toDecimals, MidpointRounding.AwayFromZero);
        }

        static object Sign(List<CalcExpression> p)
        {
            return Math.Sign((double)p[0]);
        }

        static object Sin(List<CalcExpression> p)
        {
            return Math.Sin((double)p[0]);
        }

        static object Sinh(List<CalcExpression> p)
        {
            return Math.Sinh((double)p[0]);
        }

        static object Sqrt(List<CalcExpression> p)
        {
            return Math.Sqrt((double)p[0]);
        }

        static object Tan(List<CalcExpression> p)
        {
            return Math.Tan((double)p[0]);
        }

        static object Tanh(List<CalcExpression> p)
        {
            return Math.Tanh((double)p[0]);
        }

        static object Trunc(List<CalcExpression> p)
        {
            return (double)Math.Truncate((double)p[0]);
        }

        static object Weighted(List<CalcExpression> p)
        {
            return p[0] * p[1];
        }
    }
}
