using System;
using Xunit;

namespace CalcEngineTest.Funciton
{
    public class StatisticalFunctionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public StatisticalFunctionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Theory]
        [InlineData("Average(1, 3, 3, 1, true, false, \"hello\")", 2.0)]
        [InlineData("AverageA(1, 3, 3, 1, true, false, \"hello\")", (1 + 3 + 3 + 1 + 1 + 0 + 0) / 7.0)]
        public void ShouldAverageExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Count(1, 3, 3, 1, true, false, \"hello\")", 4)]
        [InlineData("CountA(1, 3, 3, 1, true, false, \"hello\")", 7)]
        public void ShouldCountExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
