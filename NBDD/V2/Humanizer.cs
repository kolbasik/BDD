using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBDD.V2
{
    internal static class Humanizer
    {
        public static string ToString<T>(Expression<Func<T, Scenario, Task>> expression)
        {
            return ToString((LambdaExpression)expression);
        }

        public static string ToString<T>(Expression<Action<T, Scenario>> expression)
        {
            return ToString((LambdaExpression) expression);
        }

        public static string ToString(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression == null) throw new ArgumentNullException(nameof(lambdaExpression));

            var methodCallExpression = lambdaExpression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                var method = methodCallExpression.Method;
                var description = method.GetCustomAttribute<DescriptionAttribute>();
                if (description != null)
                {
                    return description.Description;
                }
                return Humanize(method.Name);
            }

            throw new NotSupportedException(string.Format(@"The format of expression is not supported. {0}", lambdaExpression));
        }

        public static string Humanize(string title)
        {
            var humanized = title ?? string.Empty;
            humanized = Regex.Replace(humanized, @"([A-Z][a-z]*)", @"$1 ").ToLowerInvariant();
            humanized = Regex.Replace(humanized, @"(\s|_)+", @" ").Trim();
            return humanized;
        }

        public static string Prefix(string prefix, string title)
        {
            var result = title ?? string.Empty;
            if (string.IsNullOrEmpty(prefix) || result.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var chars = result.ToCharArray();
                if (chars.Length > 0 && char.IsUpper(chars[0]) == false)
                {
                    chars[0] = char.ToUpper(chars[0], CultureInfo.InvariantCulture);
                    result = new string(chars);
                }
            }
            else
            {
                result = prefix + result;
            }
            return result;
        }
    }
}