using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace CalcEngineTest.Expression
{
    public class CalcBindingExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;
        private Student student;

        public CalcBindingExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
            calcEngine.DataContext = student = TestData.GetStudent();
        }

        [Fact]
        public void ShouldNameEquals()
        {
            var result = calcEngine.Evaluate("Name");
            Assert.Equal(student.Name, result);
        }

        [Fact]
        public void ShouldNameLengthEquals()
        {
            calcEngine.OptimizeExpressions = false;

            var result = calcEngine.Evaluate("Name.Length * 2");
            Assert.Equal(student.Name.Length * 2, int.Parse(result.ToString()));
        }

        [Fact]
        public void ShouldAddressEquals()
        {
            var result = calcEngine.Evaluate("Address");
            Assert.Equal(student.Address, result);
        }

        [Fact]
        public void ShouldAddressCityEquals()
        {
            var result = calcEngine.Evaluate("Address.City");
            Assert.Equal(student.Address.City, result);
        }

        [Fact]
        public void ShouldAgeFunctionEquals()
        {
            var result = calcEngine.Evaluate("IF(Age>20,Age*0.8,Age)");
            Assert.Equal(student.Age > 20 ? student.Age * 0.8 : student.Age, (int)result);
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
