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

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
