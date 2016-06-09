using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class ScenarioExtensions
    {
        [DebuggerHidden]
        public static Scenario Prop<T>(this Scenario scenario, string name, T value)
        {
            scenario.Props[name] = value;
            return scenario;
        }

        [DebuggerHidden]
        public static T Prop<T>(this Scenario scenario, string name)
        {
            object value;
            if (scenario.Props.TryGetValue(name, out value))
            {
                if (value is T)
                {
                    return (T) value;
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T);
        }

        [DebuggerHidden]
        public static Scenario Bind(this Scenario scenario, Action action)
        {
            return scenario.Bind(action.AsAsync());
        }

        [DebuggerHidden]
        public static Scenario Bind(this Scenario scenario, Action<Scenario> action)
        {
            return scenario.Bind(action.Bind(scenario).AsAsync());
        }

        [DebuggerHidden]
        public static Scenario Bind(this Scenario scenario, Func<Scenario, Task> action)
        {
            return scenario.Bind(action.Bind(scenario));
        }

        [DebuggerHidden]
        public static Scenario Step(this Scenario scenario, string title, Action action)
        {
            return scenario.Step(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Scenario Step(this Scenario scenario, string title, Action<Scenario> action)
        {
            return scenario.Step(title, action.Bind(scenario).AsAsync());
        }

        [DebuggerHidden]
        public static Scenario Step(this Scenario scenario, string title, Func<Scenario, Task> action)
        {
            return scenario.Step(title, action.Bind(scenario));
        }

        [DebuggerHidden]
        public static string Transform(this Scenario scenario, string text)
        {
            var title = Regex.Replace(text, @"\$(?<Prop>\w+)",
                delegate(Match match)
                {
                    var name = match.Groups[@"Prop"].Value;
                    return scenario.Prop<object>(name)?.ToString() ?? match.Value;
                });
            return title;
        }
    }
}