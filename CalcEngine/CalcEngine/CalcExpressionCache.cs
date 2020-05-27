using System;
using System.Collections.Generic;
using CalcEngine.Expressions;

namespace CalcEngine
{
    /// <summary>
    /// 计算缓存，每次计算的表达式存储
    /// </summary>
    internal class CalcExpressionCache
    {
        Dictionary<string, WeakReference> _dct;
        CalcEngine _ce;
        int _hitCount;

        public CalcExpressionCache(CalcEngine ce)
        {
            _ce = ce;
            _dct = new Dictionary<string, WeakReference>();
        }

        public CalcExpression this[string expression]
        {
            get
            {
                CalcExpression x = null;
                WeakReference wr;

                if (_dct.TryGetValue(expression, out wr))
                {
                    x = wr.Target as CalcExpression;
                }

                if (x == null)
                {
                    if (_dct.Count > 100 && _hitCount++ > 100)
                    {
                        RemoveDeadReferences();
                        _hitCount = 0;
                    }

                    x = _ce.Parse(expression);
                    _dct[expression] = new WeakReference(x);
                }

                return x;
            }
        }

        void RemoveDeadReferences()
        {
            for (bool done = false; !done;)
            {
                done = true;
                foreach (var k in _dct.Keys)
                {
                    if (!_dct[k].IsAlive)
                    {
                        _dct.Remove(k);
                        done = false;
                        break;
                    }
                }
            }
        }
    }
}
