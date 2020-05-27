using System;
using System.Collections.Generic;
using System.Text;

namespace CalcEngineTest.Expression
{
    public class CalcVariableExpressionTest : IDisposable
    {
        private CalcEngine.CalcEngine calcEngine;

        public CalcVariableExpressionTest()
        {
            calcEngine = new CalcEngine.CalcEngine();
        }


        public void Dispose()
        {
            calcEngine = null;
        }
    }
}
