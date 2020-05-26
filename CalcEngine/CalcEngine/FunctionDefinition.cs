using System.Collections.Generic;
using CalcEngine.Expressions;

namespace CalcEngine
{
    /// <summary>
    /// Delegate that represents CalcEngine functions.
    /// </summary>
    /// <param name="parms">List of <see cref="Expression"/> objects that represent the
    /// parameters to be used in the function call.</param>
    /// <returns>The function result.</returns>
    public delegate object CalcEngineFunction(List<CalcExpression> parms);

    /// <summary>
    /// Function definition class (keeps function name, parameter counts, and delegate).
    /// </summary>
    public class FunctionDefinition
    {
        // ** fields
        public int ParmMin, ParmMax;
        public CalcEngineFunction Function;

        // ** ctor
        public FunctionDefinition(int parmMin, int parmMax, CalcEngineFunction function)
        {
            ParmMin = parmMin;
            ParmMax = parmMax;
            Function = function;
        }
    }
}
