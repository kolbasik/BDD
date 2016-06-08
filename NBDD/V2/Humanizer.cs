using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBDD.V2
{
    internal static class Humanizer
    {
        public static string ToString<T>(Expression<Func<T, Scenario, Task>> expression)
        {
            return ToString((LambdaExpression) expression);
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
                var arguments = methodCallExpression.Arguments;
                var parameters = method.GetParameters();

                var kvps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < arguments.Count; i++)
                {
                    var argument = arguments[i];
                    var propCall = argument as MethodCallExpression;
                    if (propCall != null && string.Equals(@"Prop", propCall.Method.Name) &&
                        propCall.Method.IsDefined(typeof (ExtensionAttribute), true))
                    {
                        if (propCall.Arguments.Count == 2)
                        {
                            var constant = propCall.Arguments[1] as ConstantExpression;
                            if (constant != null)
                            {
                                kvps[parameters[i].Name] = (string) constant.Value;
                            }
                        }
                    }
                    else
                    {
                        var unaryExpression = argument as UnaryExpression;
                        if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                        {
                            argument = unaryExpression.Operand;
                        }
                        var dictCall = argument as MethodCallExpression;
                        if (dictCall != null && string.Equals(@"get_Item", dictCall.Method.Name))
                        {
                            var member = dictCall.Object as MemberExpression;
                            if (member != null && string.Equals(@"Props", member.Member.Name))
                            {
                                if (dictCall.Arguments.Count == 1)
                                {
                                    var constant = dictCall.Arguments[0] as ConstantExpression;
                                    if (constant != null)
                                    {
                                        kvps[parameters[i].Name] = (string) constant.Value;
                                    }
                                }
                            }
                        }
                    }
                }

                string title;
                var description = method.GetCustomAttribute<DescriptionAttribute>();
                if (description != null)
                {
                    title = description.Description;
                    title = Regex.Replace(title, @"\$(?<name>\w+)", match =>
                    {
                        string value;
                        return kvps.TryGetValue(match.Groups[@"name"].Value, out value)
                            ? @"$" + value
                            : match.Value;
                    });
                }
                else
                {
                    title = Humanize(method.Name);
                    if (kvps.Count > 0)
                    {
                        var values = kvps.Select(x => $"{x.Key}='${x.Value}'");
                        title = $@"{title}: {string.Join(@", ", values)}";
                    }
                }
                return title;
            }
            var message = string.Format(@"The format of expression is not supported. {0}", lambdaExpression);
            throw new NotSupportedException(message);
        }

        public static string Humanize(string title)
        {
            var humanized = title ?? string.Empty;
            humanized = Regex.Replace(humanized, @"([A-Z][a-z]*)", @" $1").ToLowerInvariant();
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