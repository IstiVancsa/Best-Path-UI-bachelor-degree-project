using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Models.Extension
{
    public static class Extensions
    {
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
        Dictionary<ParameterExpression, ParameterExpression> Map { get; }
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) { Map = map; }

        protected override Expression VisitParameter(ParameterExpression p)
            => base.VisitParameter(Map.GetElementOrDefault(p) ?? p);

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression expression)
            => new ParameterRebinder(map).Visit(expression);
    }
}
