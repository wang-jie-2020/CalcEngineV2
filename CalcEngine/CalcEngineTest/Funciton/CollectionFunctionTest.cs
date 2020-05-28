using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CalcEngineTest.Funciton
{
    public class CollectionFunctionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;
        private List<Student> students;

        public CollectionFunctionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
            calcEngine.DataContext = students = TestData.GetStudents();
        }

        [Fact]
        public void ShouldItemWhereExpressionEquals()
        {
            var result = calcEngine.Evaluate("WHERE(Item,Score>90)");
            Assert.Equal(students.Where(o => o.Score > 90), result);
        }

        [Fact]
        public void ShouldItemSelectExpressionEquals()
        {
            var result = calcEngine.Evaluate("SELECT(Item,Score)");
            Assert.Equal(students.Select(o => o.Score), result);
        }

        [Fact]
        public void ShouldItemSelectWhereExpressionEquals()
        {
            var result = calcEngine.Evaluate("SELECT(WHERE(Item,Score>90),Score)");
            Assert.Equal(students.Where(o => o.Score > 90).Select(o => o.Score), result);
        }

        [Fact]
        public void ShouldSumItemSelectWhereExpressionEquals()
        {
            var result = calcEngine.Evaluate("SUM(SELECT(WHERE(Item,Score>90),Score))");
            Assert.Equal(students.Where(o => o.Score > 90).Select(o => o.Score).Sum(), decimal.Parse(result.ToString()));
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
