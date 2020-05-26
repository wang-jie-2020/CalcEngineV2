using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CalcEngine.Expressions
{
    class CalcBindingExpression : CalcExpression
    {
        CalcEngine _ce;
        CultureInfo _ci;
        List<BindingInfo> _bindingPath;

        // ** ctor
        internal CalcBindingExpression(CalcEngine engine, List<BindingInfo> bindingPath, CultureInfo ci)
        {
            _ce = engine;
            _bindingPath = bindingPath;
            _ci = ci;
        }

        // ** object model
        override public object Evaluate()
        {
            return GetValue(_ce.DataContext);
        }

        // ** implementation
        object GetValue(object obj)
        {
            const BindingFlags bf =
                BindingFlags.IgnoreCase |
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.Static;

            if (obj != null)
            {
                foreach (var bi in _bindingPath)
                {
                    // get property
                    if (bi.PropertyInfo == null)
                    {
                        bi.PropertyInfo = obj.GetType().GetProperty(bi.Name, bf);
                    }

                    // get object
                    try
                    {
                        obj = bi.PropertyInfo.GetValue(obj, null);
                    }
                    catch
                    {
                        // REVIEW: is this needed?
                        System.Diagnostics.Debug.Assert(false, "shouldn't happen!");
                        bi.PropertyInfo = obj.GetType().GetProperty(bi.Name, bf);
                        bi.PropertyInfoItem = null;
                        obj = bi.PropertyInfo.GetValue(obj, null);
                    }

                    // handle indexers (lists and dictionaries)
                    if (bi.Parms != null && bi.Parms.Count > 0)
                    {
                        // get indexer property (always called "Item")
                        if (bi.PropertyInfoItem == null)
                        {
                            bi.PropertyInfoItem = obj.GetType().GetProperty("Item", bf);
                        }

                        // get indexer parameters
                        var pip = bi.PropertyInfoItem.GetIndexParameters();
                        var list = new List<object>();
                        for (int i = 0; i < pip.Length; i++)
                        {
                            var pv = bi.Parms[i].Evaluate();
                            pv = Convert.ChangeType(pv, pip[i].ParameterType, _ci);
                            list.Add(pv);
                        }

                        // get value
                        obj = bi.PropertyInfoItem.GetValue(obj, list.ToArray());
                    }
                }
            }

            // all done
            return obj;
        }
    }

    /// <summary>
    /// Helper used for building BindingExpression objects.
    /// </summary>
    class BindingInfo
    {
        public BindingInfo(string member, List<CalcExpression> parms)
        {
            Name = member;
            Parms = parms;
        }

        public string Name { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public PropertyInfo PropertyInfoItem { get; set; }

        public List<CalcExpression> Parms { get; set; }
    }
}
