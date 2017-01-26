using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleJsonApi.Extensions
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TResource>(this Expression<Func<TResource, object>> expression)
        {
            var memberExpression = (expression.Body as MemberExpression) ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;
            if (memberExpression == null) return null;
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new Exception($"'{memberExpression.Member.Name}' is not a valid property on type {typeof(TResource).Name}");
            return propertyInfo;
        }
    }
}
