using System.Linq.Expressions;
using System.Reflection;

namespace Taks.Core.Common
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>>? ConstructAndExpressionTree<T>(List<ExpressionFilter> filters)
        {
            if (filters.Count == 0)
            {
                return null;
            }

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp;
            if (filters.Count == 1)
            {
                exp = GetExpression<T>(param, filters[0]);
            }
            else
            {
                exp = GetExpression<T>(param, filters[0]);
                for (int i = 1; i < filters.Count; i++)
                {
                    exp = Expression.Or(exp, GetExpression<T>(param, filters[i]));
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        public static Expression GetExpression<T>(ParameterExpression param, ExpressionFilter filter)
        {
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
            MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

            MemberExpression member = Expression.Property(param, filter.PropertyName);
            ConstantExpression constant = Expression.Constant(filter.Value);

            return filter.Comparison switch
            {
                Comparison.Equal => Expression.Equal(member, constant),
                Comparison.GreaterThan => Expression.GreaterThan(member, constant),
                Comparison.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
                Comparison.LessThan => Expression.LessThan(member, constant),
                Comparison.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
                Comparison.NotEqual => Expression.NotEqual(member, constant),
                Comparison.Contains => Expression.Call(member, containsMethod, constant),
                Comparison.StartsWith => Expression.Call(member, startsWithMethod, constant),
                Comparison.EndsWith => Expression.Call(member, endsWithMethod, constant),
                _ => null,
            };
        }
    }
}
