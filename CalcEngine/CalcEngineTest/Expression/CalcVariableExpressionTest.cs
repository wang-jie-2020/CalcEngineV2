using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Xunit;

namespace CalcEngineTest.Expression
{
    public class CalcVariableExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcVariableExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
            calcEngine.Variables = TestData.GetDictory();
        }

        [Theory]
        [InlineData("key1", "value1")]
        public void ShouldVariableExpressionEquals(string expression, object expected)
        {
            var result = calcEngine.Evaluate(expression);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldVariableCalExpressionEquals()
        {
            var calcDict = new CalcDictionary(calcEngine);
            calcEngine.Variables = calcDict;

            calcDict["Amount"] = 12;
            calcDict["OfferPrice"] = 12.32;
            calcDict["Item1"] = "=Amount * OfferPrice";
            calcDict["Item2"] = "=Item1 * 0.06";

            Assert.Equal(8.8704, calcEngine.Evaluate("Item2"));
        }


        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
