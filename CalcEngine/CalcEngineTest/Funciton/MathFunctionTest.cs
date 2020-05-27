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

        [Theory]
        [InlineData("PI", Math.PI)]
        [InlineData("POWER(2,4)", 16.0)]
        [InlineData("ABS(12)", 12.0)]
        [InlineData("ABS(-12)", 12.0)]

        [InlineData("CEILING(1)", 1.0)]
        [InlineData("CEILING(1.1)", 2.0)]
        [InlineData("CEILING(1.6)", 2.0)]
        [InlineData("CEILING(-1)", -1.0)]
        [InlineData("CEILING(-1.1)", -1.0)]
        [InlineData("CEILING(-1.6)", -1.0)]

        [InlineData("FLOOR(1)", 1.0)]
        [InlineData("FLOOR(1.1)", 1.0)]
        [InlineData("FLOOR(1.6)", 1.0)]
        [InlineData("FLOOR(-1)", -1.0)]
        [InlineData("FLOOR(-1.1)", -2.0)]
        [InlineData("FLOOR(-1.6)", -2.0)]

        [InlineData("INT(1)", 1.0)]
        [InlineData("INT(1.1)", 1.0)]
        [InlineData("INT(1.6)", 1.0)]
        [InlineData("INT(-1)", -1.0)]
        [InlineData("INT(-1.1)", -1.0)]
        [InlineData("INT(-1.6)", -1.0)]

        [InlineData("ROUND(1.5)", 2.0)]
        [InlineData("ROUND(2.5)", 2.0)]
        [InlineData("ROUND(-1.5)", -2.0)]
        [InlineData("ROUND(-2.5)", -2.0)]
        [InlineData("ROUNDE(1.5)", 2.0)]
        [InlineData("ROUNDE(2.5)", 3.0)]
        [InlineData("ROUNDE(-1.5)", -2.0)]
        [InlineData("ROUNDE(-2.5)", -3.0)]
        public void ShouldMathExpressionEquals(string expression, object expected)
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
