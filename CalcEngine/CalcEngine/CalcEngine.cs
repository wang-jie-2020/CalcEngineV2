using CalcEngine.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CalcEngine
{
    public partial class CalcEngine
    {
        public CalcEngine()
        {
            CultureInfo = CultureInfo.InvariantCulture;

            _cache = new CalcExpressionCache(this);

            _tks = GetSymbolTable();
            _fns = GetFunctionTable();
        }

        /// <summary>
        /// 本地化
        /// </summary>
        public CultureInfo CultureInfo
        {
            get
            {
                return _ci;
            }
            set
            {
                _ci = value;

                _decimal = _ci.NumberFormat.NumberDecimalSeparator[0];
                _percent = _ci.NumberFormat.PercentSymbol[0];
                _listSep = _ci.TextInfo.ListSeparator[0];
            }
        }

        /// <summary>
        /// 外部数据源-变量字典
        /// </summary>
        public IDictionary<string, object> Variables { get; set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 外部数据源-数据上下文
        /// </summary>
        public object DataContext { get; set; }

        /// <summary>
        /// 是否缓存解析的表达式，默认缓存
        /// </summary>
        public bool CacheExpressions
        {
            get { return _cache != null; }
            set
            {
                if (value != CacheExpressions)
                {
                    _cache = value
                        ? new CalcExpressionCache(this)
                        : null;
                }
            }
        }

        /// <summary>
        /// 解析表达式时是否进行优化
        /// </summary>
        public bool OptimizeExpressions { get; set; } = true;

        /// <summary>
        /// 前缀忽略字符，比如':'
        /// </summary>
        public string IdentifierChars
        {
            get { return _idChars; }
            set { _idChars = value; }
        }

        /// <summary>
        /// 引擎中的功能
        /// </summary>
        public Dictionary<string, FunctionDefinition> Functions => _fns;

        #region 注册功能

        /// <summary>
        /// 向引擎中注册功能
        /// </summary>
        /// <param name="functionName">功能名词，不能重复</param>
        /// <param name="parmCount">参数个数</param>
        /// <param name="fn">功能委托</param>
        public void RegisterFunction(string functionName, int parmCount, CalcEngineFunction fn)
        {
            RegisterFunction(functionName, parmCount, parmCount, fn);
        }

        /// <summary>
        /// 向引擎中注册功能
        /// </summary>
        /// <param name="functionName">功能名词，不能重复</param>
        /// <param name="parmMin">最少参数个数</param>
        /// <param name="parmMax">最多参数个数，最多int.MaxValue</param>
        /// <param name="fn"></param>
        public void RegisterFunction(string functionName, int parmMin, int parmMax, CalcEngineFunction fn)
        {
            _fns.Add(functionName, new FunctionDefinition(parmMin, parmMax, fn));
        }

        #endregion

        /// <summary>
        /// 通过identifier从实现CalcEngine.Expressions.IValueObject的类型集合中查对象
        /// 比如Excel时，'A1'这个Cell的获取方法
        /// </summary>
        /// <param name="identifier">唯一标识</param>
        /// <returns></returns>
        public virtual object GetExternalObject(string identifier)
        {
            return null;
        }
    }
}
