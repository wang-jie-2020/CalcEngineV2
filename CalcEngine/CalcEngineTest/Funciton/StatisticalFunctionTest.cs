using CalcEngine.Expressions;
using System;
using System.Linq;
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
        [InlineData("Count(1, 3, 3, 1, true, false, \"hello\")", 4.0)]
        [InlineData("CountA(1, 3, 3, 1, true, false, \"hello\")", 7.0)]
        public void ShouldCountExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MIN(1, 3, 3, 1, true, false, \"hello\")", 1.0)]
        [InlineData("MINA(1, 3, 3, 1, true, false, \"hello\")", 0.0)]
        public void ShouldMinExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("MAX(1, 3, 3, 1, true, false, \"hello\")", 3.0)]
        [InlineData("MAXA(1, 3, 3, 1, true, false, \"hello\")", 3.0)]
        public void ShouldMaxExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("SUM(1, 3, 3, 1)", 8.0)]
        public void ShouldSumExpressionEquals(string expression, double expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldSumExpression2Equals()
        {
            var calcDict = new CalcDictionary(calcEngine);
            calcEngine.Variables = calcDict;

            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            calcDict["Arr"] = arr;
            calcDict["SumArr"] = "=SUM(Arr)";

            Assert.Equal(arr.Sum(), int.Parse(calcEngine.Evaluate("SumArr").ToString()));
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
