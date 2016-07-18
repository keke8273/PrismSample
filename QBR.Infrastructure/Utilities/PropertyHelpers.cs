using System;
using System.Linq.Expressions;
using System.Reflection;

namespace QBR.Infrastructure.Utilities
{
    public static class PropertyHelpers
    {
        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("Null expression");

            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression == null)
                throw new ArgumentException("Please provide a lambda expression like 'n => n.PropertyName'");

            MemberInfo memberInfo = memberExpression.Member;

            if (String.IsNullOrEmpty(memberInfo.Name))
                throw new ArgumentException("'expression' did not provide a property name.");

            return memberInfo.Name;
        }
    }
}
