using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace CalcEngine.Expressions
{
    /// <summary>
    /// 属性访问表达式
    /// </summary>
    internal class CalcBindingExpression : CalcExpression
    {
        private readonly innerDataSource _innerData;
        private readonly List<BindingInfo> _bindingPath;
        private readonly CultureInfo _ci;

        internal CalcBindingExpression(innerDataSource innerData, List<BindingInfo> bindingPath, CultureInfo ci)
        {
            _innerData = innerData;
            _bindingPath = bindingPath;
            _ci = ci;
        }

        public override object Evaluate()
        {
            return GetValue(_innerData.DataContext);
        }

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
                    if (bi.PropertyInfo == null)
                    {
                        bi.PropertyInfo = obj.GetType().GetProperty(bi.Name, bf);
                    }

                    try
                    {
                        obj = bi.PropertyInfo.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        //System.Diagnostics.Debug.Assert(false, "shouldn't happen!");
                    }

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

    internal class BindingInfo
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
