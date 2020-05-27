using System;
using System.Collections.Generic;
using System.Text;

namespace CalcEngineTest.Expression
{
    public class CalcFunctionExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcFunctionExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }


        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
