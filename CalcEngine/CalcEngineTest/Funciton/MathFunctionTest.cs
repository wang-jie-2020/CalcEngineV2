using System;
using System.Collections.Generic;
using Xunit;

namespace CalcEngineTest.Funciton
{
    public class MathFunctionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public MathFunctionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Fact]
        public void ShouldEquals()
        {
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();

            keyValuePairs.Add("ABS(-12)", 12.0);
            keyValuePairs.Add("ABS(+12)", 12.0);
            keyValuePairs.Add("CEILING(1.8)", Math.Ceiling(1.8));
            keyValuePairs.Add("INT(1.8)", 1);
            keyValuePairs.Add("FLOOR(1.8)", Math.Floor(1.8));
            keyValuePairs.Add("POWER(2,4)", Math.Pow(2, 4));
            keyValuePairs.Add("PI", Math.PI);

            foreach (var item in keyValuePairs)
            {
                var result = calcEngine.Evaluate(item.Key);
                Assert.Equal(item.Value, result);
            }
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
