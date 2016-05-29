using System;
using System.Text.RegularExpressions;

namespace NBDD.V2
{
    public static class ScenarioExtensions
    {
        public static Scenario Prop<T>(this Scenario scenario, string name, T value)
        {
            scenario.Props[name] = value;
            return scenario;
        }

        public static T Prop<T>(this Scenario scenario, string name)
        {
            object value;
            if (scenario.Props.TryGetValue(name, out value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }

        public static Scenario Bind(this Scenario scenario, Action<Scenario> bind)
        {
            return scenario.Bind(bind.AsAsync());
        }

        public static string Transform(this Scenario scenario, string text)
        {
            var title = Regex.Replace(text, @"\$(?<Prop>\w+)",
                delegate(Match match)
                {
                    var name = match.Groups[@"Prop"].Value;
                    return scenario.Prop<string>(name) ?? match.Value;
                });
            return title;
        }
    }
}