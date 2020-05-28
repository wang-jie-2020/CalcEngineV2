using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CalcEngineTest.Expression
{
    public class CalcUnaryExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcUnaryExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Theory]
        [InlineData("+5", 5.0)]
        [InlineData("-5", -5.0)]
        public void ShouldUnaryExpressionEquals(string expression, object expected)
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
