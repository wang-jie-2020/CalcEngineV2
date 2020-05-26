using System.Collections;
using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    class CalcXObjectExpression : CalcExpression, IEnumerable
    {
        object _value;

        // ** ctor
        internal CalcXObjectExpression(object value)
        {
            _value = value;
        }

        // ** object model
        public override object Evaluate()
        {
            // use IValueObject if available
            var iv = _value as IValueObject;
            if (iv != null)
            {
                return iv.GetValue();
            }

            // return raw object
            return _value;
        }
        public IEnumerator GetEnumerator()
        {
            var ie = _value as IEnumerable;
            return ie != null ? ie.GetEnumerator() : null;
        }
    }

    /// <summary>
    /// Interface supported by external objects that have to return a value
    /// other than themselves (e.g. a cell range object should return the 
    /// cell content instead of the range itself).
    /// </summary>
    public interface IValueObject
    {
        object GetValue();
    }
}
