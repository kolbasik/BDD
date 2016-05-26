using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NBDD.V2
{
    public static class Bdd
    {
        public enum Stage
        {
            Given,
            When,
            Then
        }

        public static Scenario<TSpec> Scen<TSpec>() where TSpec : new()
        {
            return Scen(new TSpec());
        }

        public static Scenario<TSpec> Scen<TSpec>(TSpec spec)
        {
            return new Scenario<TSpec>(spec,
                delegate(ScenarioSteps x)
                {
                    Trace.WriteLine(x.Steps.Count);
                    foreach (var step in x.Steps)
                    {
                        Trace.WriteLine(step.Title);
                        step.Action.Invoke();
                    }
                });
        }

        [AttributeUsage(AttributeTargets.Method)]
        public abstract class StepAttribute : Attribute
        {
            public Stage Stage => EnumX.Parse<Stage>(GetType().Name.Replace(@"Attribute", string.Empty));
            public string Title { get; set; }
        }

        public sealed class GivenAttribute : StepAttribute
        {
        }

        public sealed class WhenAttribute : StepAttribute
        {
        }

        public sealed class ThenAttribute : StepAttribute
        {
        }

        public sealed class Scenario<TSpec>
        {
            private readonly TSpec spec;
            private readonly Action<ScenarioSteps> execute;
            private readonly ScenarioSteps steps;
            private Stage stage;

            internal Scenario(TSpec spec, Action<ScenarioSteps> execute)
            {
                if (spec == null)
                {
                    throw new ArgumentNullException(nameof(spec));
                }
                if (execute == null)
                {
                    throw new ArgumentNullException(nameof(execute));
                }
                this.spec = spec;
                this.execute = execute;
                this.stage = Stage.Given;
                this.steps = new ScenarioSteps();
            }

            public Scenario<TSpec> Given(Expression<Action<TSpec>> expression)
            {
                Append(Stage.Given, expression);
                return this;
            }

            public Scenario<TSpec> When(Expression<Action<TSpec>> expression)
            {
                if (stage == Stage.Given)
                {
                    stage = Stage.When;
                }
                Append(Stage.When, expression);
                return this;
            }

            public Scenario<TSpec> Then(Expression<Action<TSpec>> expression)
            {
                if (stage == Stage.When)
                {
                    stage = Stage.Then;
                }
                Append(Stage.Then, expression);
                return this;
            }

            public Scenario<TSpec> And(Expression<Action<TSpec>> expression)
            {
                Append(stage, expression);
                return this;
            }

            public void Execute() => execute(steps);

            private void Append(Stage stage, Expression<Action<TSpec>> expression)
            {
                if (this.stage != stage)
                {
                    throw new NotSupportedException(
                        @"The scenario steps should be ordered in the following order: Given, When, Then.");
                }

                var title = StepUtils.GetTitle(expression);
                var action = StepUtils.GetAction(spec, expression);

                steps.Append(stage, title, action);
            }
        }

        [DebuggerDisplay("Steps: {Steps.Count}")]
        internal sealed class ScenarioSteps
        {
            private readonly Dictionary<Stage, List<ScenarioStep>> steps;

            public ScenarioSteps()
            {
                this.steps = new Dictionary<Stage, List<ScenarioStep>>();
                foreach (var stage in EnumX.GetValues<Stage>())
                {
                    steps.Add(stage, new List<ScenarioStep>());
                }
            }
            internal IReadOnlyCollection<ScenarioStep> Steps
                => steps.SelectMany(x => x.Value).ToList().AsReadOnly();

            public void Append(Stage stage, string title, Action action)
            {
                //var methodCallExpression = expression.Body as MethodCallExpression;
                //if (methodCallExpression == null)
                //{
                //    throw new NotSupportedException(@"The scenario step should be as a single method call.");
                //}
                //var stepAttribute = methodCallExpression.Method.GetCustomAttribute<StepAttribute>();
                //if (stepAttribute == null)
                //{
                //    throw new NotSupportedException(
                //        @"The spec step should be defined with one of Given, When or Then attribute.");
                //}
                //if (stepAttribute.Stage != stage)
                //{
                //    throw new NotSupportedException(@"The scenario step should have the same stage as in spec.");
                //}
                steps[stage].Add(new ScenarioStep(title, action));
            }
        }

        [DebuggerDisplay("Step: {Title}")]
        internal sealed class ScenarioStep
        {
            public ScenarioStep(string title, Action action)
            {
                Title = title;
                Action = action;
            }

            public string Title { get; }
            public Action Action { get; }
        }

        internal static class StepUtils
        {
            public static string GetTitle<T>(Expression<Action<T>> expression)
            {
                var methodCallExpression = expression.Body as MethodCallExpression;
                var stepAttribute = methodCallExpression?.Method.GetCustomAttribute<StepAttribute>();
                if (!string.IsNullOrWhiteSpace(stepAttribute?.Title))
                {
                    return stepAttribute.Title;
                }
                return methodCallExpression?.Method.Name.Replace("__", "_$").Replace("_", " ");
            }

            public static Action GetAction<T>(T target, Expression<Action<T>> expression)
            {
                var action = expression.Compile();
                return () => action.Invoke(target);
            }
        }
    }

    internal static class EnumX
    {
        public static T Parse<T>(string value) => (T) Enum.Parse(typeof (T), value, true);
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof (T)).OfType<T>();
    }
}