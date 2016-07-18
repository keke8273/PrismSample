using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace QBR.Infrastructure.Utilities
{
    /// <summary>
    /// Extension methods for the string class to parse out the name of the property name
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyLambda">The property lambda.</param>
        /// <returns>
        /// The name of the property.
        /// </returns>
        /// <exception cref="System.Data.InvalidExpressionException"></exception>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {

            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new InvalidExpressionException();
            }

            return me.Member.Name;
        }

        /// <summary>
        /// Gets the display name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyLambda">The property lambda.</param>
        /// <returns>
        /// The displayable name of the property.
        /// </returns>
        public static string GetDisplayPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var propertyName = GetPropertyName(propertyLambda);

            return GetDisplayString(propertyName);
        }

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        public static string GetDisplayString(string displayName)
        {
            for (var i = 0; i < displayName.ToCharArray().Length - 1; i++)
            {
                if (Char.IsLower(displayName[i]) && Char.IsUpper(displayName[i + 1]))
                {
                    displayName = displayName.Insert(i + 1, " ");
                }
            }
            return displayName;
        }

        /// <summary>
        /// Parses the input string or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T ParseOrDefault<T>(this T targetType, string source) where T : new()
        {
            if (targetType.GetType().GetMethods(BindingFlags.Static | BindingFlags.Public).Any(methodInfo => methodInfo.Name == "TryParse"))
            {
                var arguements = new object[] { source, null };

                var parameterTypes = new[] { source.GetType(), targetType.GetType().MakeByRefType() };

                var tryParseMethod = targetType.GetType().GetMethod("TryParse", parameterTypes);

                var retval = (bool)tryParseMethod.Invoke(null, arguements);

                if (retval)
                    return (T)arguements[1];
            }

            return new T();
        }
    }
}