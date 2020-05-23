using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Utils
{
    public static class Extensions
    {
        //TODO check if has the specific profile
        //public static bool HasProfiles(this IDictionary<string, object> applicationProperties, params TProfile[] profile)
        //{
        //    if (applicationProperties.ContainsKey("UserType"))
        //        for (int i = 0; i < profile.Count(); i++)
        //            if (Convert.ToString(profile[i]).ToLower() == Convert.ToString(applicationProperties["UserType"]).ToLower())
        //                return true;
        //    return false;
        //}

        //used to add default value for drop down TODO we might not need this
        public static IList<string> AddFilterNullValue(this IEnumerable<string> items, string Name)
        {
            var list = items.ToList();
            list.Insert(0, Name);
            return list;
        }

        public static TValue GetElementOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            var result = default(TValue);
            dict.TryGetValue(key, out result);
            return result;
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters
                .Zip(second.Parameters, (f, s) => new { f, s })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private Dictionary<ParameterExpression, ParameterExpression> Map { get; }
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) { this.Map = map; }

        protected override Expression VisitParameter(ParameterExpression p)
            => base.VisitParameter(this.Map.GetElementOrDefault(p) ?? p);

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression expression)
            => new ParameterRebinder(map).Visit(expression);
    }
}
