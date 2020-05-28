using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Xunit;

namespace CalcEngineTest.Expression
{
    public class CalcBindingExpressionTest2 : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;
        private List<Student> students;

        public CalcBindingExpressionTest2()
        {
            calcEngine = new CalcEngine.CalcEngine();
            calcEngine.DataContext = students = TestData.GetStudents();
        }

        [Fact]
        public void ShouldCapacityEquals()
        {
            var result = calcEngine.Evaluate("capacity");
            Assert.Equal(students.Capacity, result);
        }

        [Fact]
        public void ShouldItemEquals()
        {
            var result = calcEngine.Evaluate("Item(0)");
            Assert.Equal(students[0], result);
        }


        //[Fact]
        //public void ShouldScoreSumEquals()
        //{
        //    var result = calcEngine.Evaluate("SUM(Score)");
        //    Assert.Equal(students.Sum(o => o.Score), result);
        //}

        //[Fact]
        //public void ShouldAddressCityConcatEquals()
        //{
        //    var result = calcEngine.Evaluate("Concat(Address.City)");
        //    Assert.Equal(string.Concat(students.Select(p => p.Address.City)), result);
        //}

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
