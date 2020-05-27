using System;
using System.Collections.Generic;
using System.Text;
using CalcEngine.Expressions;
using CalcEngine.Functions.InternalFunction;

namespace CalcEngine.Functions
{
    /// <summary>
    /// 统计学功能
    /// </summary>
    public static class StatisticalFunction
    {
        public static void Register(CalcEngine ce)
        {
            ce.RegisterFunction("AVERAGE", 1, int.MaxValue, Average);
            ce.RegisterFunction("AVERAGEA", 1, int.MaxValue, AverageA);

            ce.RegisterFunction("COUNT", 1, int.MaxValue, Count);
            ce.RegisterFunction("COUNTA", 1, int.MaxValue, CountA);
            //ce.RegisterFunction("COUNTBLANK", 1, int.MaxValue, CountBlank);
            //ce.RegisterFunction("COUNTIF", 2, CountIf);

            ce.RegisterFunction("MIN", 1, int.MaxValue, Min);
            ce.RegisterFunction("MINA", 1, int.MaxValue, MinA);

            ce.RegisterFunction("MAX", 1, int.MaxValue, Max);
            ce.RegisterFunction("MAXA", 1, int.MaxValue, MaxA);

            ce.RegisterFunction("MEDIAN", 1, int.MaxValue, Median);
            ce.RegisterFunction("RANGE", 1, int.MaxValue, Range);

            ce.RegisterFunction("SUM", 1, int.MaxValue, Sum);
            //ce.RegisterFunction("SUMIF", 2, 3, SumIf);

            ce.RegisterFunction("STDEV", 1, int.MaxValue, StDev);
            ce.RegisterFunction("STDEVA", 1, int.MaxValue, StDevA);
            ce.RegisterFunction("STDEVP", 1, int.MaxValue, StDevP);
            ce.RegisterFunction("STDEVPA", 1, int.MaxValue, StDevPA);

            ce.RegisterFunction("VAR", 1, int.MaxValue, Var);
            ce.RegisterFunction("VARA", 1, int.MaxValue, VarA);
            ce.RegisterFunction("VARP", 1, int.MaxValue, VarP);
            ce.RegisterFunction("VARPA", 1, int.MaxValue, VarPA);
        }

        static object Average(List<CalcExpression> p)
        {
            return GetTally(p, true).Average();
        }

        static object AverageA(List<CalcExpression> p)
        {
            return GetTally(p, false).Average();
        }

        static object Count(List<CalcExpression> p)
        {
            return GetTally(p, true).Count();
        }

        static object CountA(List<CalcExpression> p)
        {
            return GetTally(p, false).Count();
        }

        //static object CountBlank(List<CalcExpression> p)
        //{
        //    var cnt = 0.0;
        //    foreach (CalcExpression e in p)
        //    {
        //        var ienum = e as System.Collections.IEnumerable;
        //        if (ienum != null)
        //        {
        //            foreach (var value in ienum)
        //            {
        //                if (IsBlank(value))
        //                    cnt++;
        //            }
        //        }
        //        else
        //        {
        //            if (IsBlank(e.Evaluate()))
        //                cnt++;
        //        }
        //    }
        //    return cnt;
        //}

        //static bool IsBlank(object value)
        //{
        //    return
        //        value == null ||
        //        value is string && ((string)value).Length == 0;
        //}

        //static object CountIf(List<CalcExpression> p)
        //{
        //    CalcEngine ce = new CalcEngine();
        //    var cnt = 0.0;
        //    var ienum = p[0] as System.Collections.IEnumerable;
        //    if (ienum != null)
        //    {
        //        var crit = (string)p[1].Evaluate();
        //        foreach (var value in ienum)
        //        {
        //            if (!IsBlank(value))
        //            {
        //                var exp = string.Format("{0}{1}", value, crit);
        //                if ((bool)ce.Evaluate(exp))
        //                    cnt++;
        //            }
        //        }
        //    }
        //    return cnt;
        //}

        static object Min(List<CalcExpression> p)
        {
            return GetTally(p, true).Min();
        }

        static object MinA(List<CalcExpression> p)
        {
            return GetTally(p, false).Min();
        }

        static object Max(List<CalcExpression> p)
        {
            return GetTally(p, true).Max();
        }

        static object MaxA(List<CalcExpression> p)
        {
            return GetTally(p, false).Max();
        }

        static object Median(List<CalcExpression> parameters)
        {
            return GetTally(parameters, true).Median();
        }

        static object Range(List<CalcExpression> p)
        {
            return GetTally(p, true).Range();
        }

        static object Sum(List<CalcExpression> p)
        {
            var tally = new Tally();
            foreach (CalcExpression e in p)
            {
                tally.Add(e);
            }
            return tally.Sum();
        }

        //static object SumIf(List<CalcExpression> p)
        //{
        //    // get parameters
        //    IEnumerable range = p[0] as IEnumerable;
        //    IEnumerable sumRange = p.Count < 3 ? range : p[2] as IEnumerable;
        //    var criteria = p[1].Evaluate();

        //    // build list of values in range and sumRange
        //    var rangeValues = new List<object>();
        //    foreach (var value in range)
        //    {
        //        rangeValues.Add(value);
        //    }
        //    var sumRangeValues = new List<object>();
        //    foreach (var value in sumRange)
        //    {
        //        sumRangeValues.Add(value);
        //    }

        //    // compute total
        //    var ce = new CalcEngine();
        //    var tally = new Tally();
        //    for (int i = 0; i < Math.Min(rangeValues.Count, sumRangeValues.Count); i++)
        //    {
        //        if (ValueSatisfiesCriteria(rangeValues[i], criteria, ce))
        //        {
        //            tally.AddValue(sumRangeValues[i]);
        //        }
        //    }

        //    // done
        //    return tally.Sum();
        //}

        //static bool ValueSatisfiesCriteria(object value, object criteria, CalcEngine ce)
        //{
        //    // safety...
        //    if (value == null)
        //    {
        //        return false;
        //    }

        //    // if criteria is a number, straight comparison
        //    if (criteria is double)
        //    {
        //        return (double)value == (double)criteria;
        //    }

        //    // convert criteria to string
        //    var cs = criteria as string;
        //    if (!string.IsNullOrEmpty(cs))
        //    {
        //        // if criteria is an expression (e.g. ">20"), use calc engine
        //        if (cs[0] == '=' || cs[0] == '<' || cs[0] == '>')
        //        {
        //            // build expression
        //            var expression = string.Format("{0}{1}", value, cs);

        //            // add quotes if necessary
        //            var pattern = @"(\w+)(\W+)(\w+)";
        //            var m = Regex.Match(expression, pattern);
        //            if (m.Groups.Count == 4)
        //            {
        //                double d;
        //                if (!double.TryParse(m.Groups[1].Value, out d) ||
        //                    !double.TryParse(m.Groups[3].Value, out d))
        //                {
        //                    expression = string.Format("\"{0}\"{1}\"{2}\"",
        //                        m.Groups[1].Value,
        //                        m.Groups[2].Value,
        //                        m.Groups[3].Value);
        //                }
        //            }

        //            // evaluate
        //            return (bool)ce.Evaluate(expression);
        //        }

        //        // if criteria is a regular expression, use regex
        //        if (cs.IndexOf('*') > -1)
        //        {
        //            var pattern = cs.Replace(@"\", @"\\");
        //            pattern = pattern.Replace(".", @"\");
        //            pattern = pattern.Replace("*", ".*");
        //            return Regex.IsMatch(value.ToString(), pattern, RegexOptions.IgnoreCase);
        //        }

        //        // straight string comparison 
        //        return string.Equals(value.ToString(), cs, StringComparison.OrdinalIgnoreCase);
        //    }

        //    // should never get here?
        //    System.Diagnostics.Debug.Assert(false, "failed to evaluate criteria in SumIf");
        //    return false;
        //}

        static object StDev(List<CalcExpression> p)
        {
            return GetTally(p, true).Std();
        }

        static object StDevA(List<CalcExpression> p)
        {
            return GetTally(p, false).Std();
        }

        static object StDevP(List<CalcExpression> p)
        {
            return GetTally(p, true).StdP();
        }

        static object StDevPA(List<CalcExpression> p)
        {
            return GetTally(p, false).StdP();
        }

        static object Var(List<CalcExpression> p)
        {
            return GetTally(p, true).Var();
        }

        static object VarA(List<CalcExpression> p)
        {
            return GetTally(p, false).Var();
        }

        static object VarP(List<CalcExpression> p)
        {
            return GetTally(p, true).VarP();
        }

        static object VarPA(List<CalcExpression> p)
        {
            return GetTally(p, false).VarP();
        }

        #region Tally

        static Tally GetTally(List<CalcExpression> p, bool numbersOnly)
        {
            var tally = new Tally(numbersOnly);
            foreach (CalcExpression e in p)
            {
                tally.Add(e);
            }
            return tally;
        }

        #endregion
    }
}
