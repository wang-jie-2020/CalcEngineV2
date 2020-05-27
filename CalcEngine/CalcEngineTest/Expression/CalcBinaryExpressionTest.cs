using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CalcEngineTest.Expression
{
    public class CalcBinaryExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcBinaryExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Theory]
        [InlineData("1+2-3+4*5+6/2+8^2", 87.0)]
        [InlineData(".5+1,000", 1000.5)]
        public void ShouldBinaryExpressionEquals(string expression, object expected)
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
