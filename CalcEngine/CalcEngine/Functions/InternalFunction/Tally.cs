using CalcEngine.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CalcEngine.Functions.InternalFunction
{
    /// <summary>
    /// 记录统计功能的相关
    /// </summary>
    internal class Tally
    {
        protected double _sum, _sum2, _cnt, _min, _max;
        protected List<double> _vals;

        protected readonly bool _numbersOnly;

        public Tally() : this(false) { }

        public Tally(bool numbersOnly)
        {
            _numbersOnly = numbersOnly;
            _vals = new List<double>();
        }

        public void Add(CalcExpression e)
        {
            if (e is IEnumerable ienum)
            {
                foreach (var value in ienum)
                {
                    AddValue(value);
                }
                return;
            }

            AddValue(e.Evaluate());
        }

        public virtual void AddValue(object value)
        {
            //非数字模式时只将非数字转为0，不跳过是统计个数的目的
            if (!_numbersOnly)
            {
                if (value == null || value is string)
                {
                    value = 0;
                }

                //true是否作为1待考虑
                if (value is bool)
                {
                    value = (bool)value ? 1 : 0;
                }
            }

            if (value != null)
            {
                var typeCode = Type.GetTypeCode(value.GetType());
                if (typeCode >= TypeCode.Char && typeCode <= TypeCode.Decimal)
                {
                    value = Convert.ChangeType(value, typeof(double), System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            if (value is double)
            {
                var dbl = (double)value;

                _vals.Add((double)value);
                _sum += dbl;
                _sum2 += dbl * dbl;
                _cnt++;

                if (_cnt == 1 || dbl < _min)
                {
                    _min = dbl;
                }
                if (_cnt == 1 || dbl > _max)
                {
                    _max = dbl;
                }
            }
        }

        public double Count() { return _cnt; }

        public double Sum() { return _sum; }

        public double Average() { return _sum / _cnt; }

        public double Min() { return _min; }

        public double Max() { return _max; }

        public double Range() { return _max - _min; }

        /// <summary>
        /// 中位数
        /// </summary>
        /// <returns></returns>
        public double Median()
        {
            var assessments = _vals.ToArray();

            if (_vals.Count <= 0)
            {
                return 0;
            }

            Array.Sort(assessments);

            var arrayLength = assessments.Length;

            if (arrayLength % 2 == 0)
            {
                return (assessments[(arrayLength / 2) - 1] + assessments[arrayLength / 2]) / 2;
            }

            return assessments[(int)Math.Floor((decimal)arrayLength / 2)];
        }

        #region 方差、标准差

        public double Var()
        {
            var avg = Average();
            return _cnt <= 1 ? 0 : (_sum2 / _cnt - avg * avg) * _cnt / (_cnt - 1);
        }

        public double Std()
        {
            var avg = Average();
            return _cnt <= 1 ? 0 : Math.Sqrt((_sum2 / _cnt - avg * avg) * _cnt / (_cnt - 1));
        }

        public double VarP()
        {
            var avg = Average();
            return _cnt <= 1 ? 0 : _sum2 / _cnt - avg * avg;
        }

        public double StdP()
        {
            var avg = Average();
            return _cnt <= 1 ? 0 : Math.Sqrt(_sum2 / _cnt - avg * avg);
        }

        #endregion
    }
}
