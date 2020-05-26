using System;
using Xunit;

namespace CalcEngineTest
{
    public class LogicalFunctionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public LogicalFunctionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Theory]
        [InlineData("AND(true, true)", true)]
        [InlineData("AND(true, false)", false)]
        [InlineData("AND(false, true)", false)]
        [InlineData("AND(false, false)", false)]
        public void ShouldAndExpressionEquals(string expression, bool expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("OR(true, true)", true)]
        [InlineData("OR(true, false)", true)]
        [InlineData("OR(false, true)", true)]
        [InlineData("OR(false, false)", false)]
        public void ShouldOrEquals(string expression, bool expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("TRUE()", true)]
        [InlineData("FALSE()", false)]
        public void ShouldExpressionEquals(string expression, bool expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("NOT(true)", false)]
        [InlineData("NOT(false)", true)]
        public void ShouldNotExpressionEquals(string expression, bool expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("IF(5 > 4, true, false)", true)]
        [InlineData("IF(5 < 4, true, false)", false)]
        public void ShouldIfExpressionEquals(string expression, bool expected)
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
