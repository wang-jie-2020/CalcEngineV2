using System.Collections;
using System.Collections.Generic;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 实现IValueObject的对象的值，比如表格Cell对象
    /// </summary>
    internal class CalcXObjectExpression : CalcExpression, IEnumerable
    {
        object _value;

        internal CalcXObjectExpression(object value)
        {
            _value = value;
        }

        public override object Evaluate()
        {
            if (_value is IValueObject iv)
            {
                return iv.GetValue();
            }

            return _value;
        }
        public IEnumerator GetEnumerator()
        {
            return _value is IEnumerable ie ? ie.GetEnumerator() : null;
        }
    }

    public interface IValueObject
    {
        object GetValue();
    }
}
