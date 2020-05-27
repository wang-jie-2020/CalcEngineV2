using System;
using System.Collections.Generic;
using System.Text;

namespace CalcEngineTest.Expression
{
    public class CalcXObjectExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcXObjectExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }

        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
