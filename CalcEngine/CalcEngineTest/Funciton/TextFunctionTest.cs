using System;
using Xunit;

namespace CalcEngineTest.Funciton
{
    public class TextFunctionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public TextFunctionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        [Theory]
        [InlineData("CHAR(65)", "A")]
        [InlineData("CODE(\"A\")", 65)]
        [InlineData("CONCAT(\"a\", \"b\")", "ab")]
        [InlineData("FIND(\"bra\", \"abracadabra\")", 2)]
        [InlineData("FIND(\"BRA\", \"abracadabra\")", -1)]
        [InlineData("LEFT(\"abracadabra\", 3)", "abr")]
        [InlineData("LEFT(\"abracadabra\")", "a")]
        [InlineData("LEN(\"abracadabra\")", 11)]
        [InlineData("LOWER(\"ABRACADABRA\")", "abracadabra")]
        [InlineData("MID(\"abracadabra\", 1, 3)", "abr")]
        [InlineData("REPLACE(\"abracadabra\", 1, 3, \"XYZ\")", "XYZacadabra")]
        [InlineData("REPT(\"abr\", 3)", "abrabrabr")]
        [InlineData("RIGHT(\"abracadabra\", 3)", "bra")]
        [InlineData("SEARCH(\"bra\", \"abracadabra\")", 2)]
        [InlineData("SEARCH(\"BRA\", \"abracadabra\")", 2)]
        [InlineData("TOSTRING(123)", "123")]
        [InlineData("TRIM(\"   hello   \")", "hello")]
        [InlineData("UPPER(\"abracadabra\")", "ABRACADABRA")]
        [InlineData("VALUE(\"1234\")", 1234.0)]
        //[InlineData("SUBSTITUTE(\"abracadabra\", \"a\", \"b\")", "bbrbcbdbbrb")]
        //[InlineData("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\")", "bbcbbcbbc")]
        //[InlineData("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\", 1)", "bbcabcabc")]
        //[InlineData("SUBSTITUTE(\"abcabcabc\", \"a\", \"b\", 2)", "abcbbcabc")]
        //[InlineData("SUBSTITUTE(\"abcabcabc\", \"A\", \"b\", 2)", "abcabcabc")]
        public void ShouldAverageExpressionEquals(string expression, object expected)
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
