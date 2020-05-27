using System;
using System.Collections.Generic;
using System.Text;

namespace CalcEngineTest.Expression
{
    public class CalcUnaryExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcUnaryExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }


        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
