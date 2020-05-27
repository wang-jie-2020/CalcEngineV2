using System.Collections.Generic;
using CalcEngine.Expressions;

namespace CalcEngine
{
    public class FunctionDefinition
    {
        public int ParmMin, ParmMax;
        public CalcEngineFunction Function;

        public FunctionDefinition(int parmMin, int parmMax, CalcEngineFunction function)
        {
            ParmMin = parmMin;
            ParmMax = parmMax;
            Function = function;
        }
    }

    public delegate object CalcEngineFunction(List<CalcExpression> parms);
}
