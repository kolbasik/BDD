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
            return new Scenario<TSpec>(x => Trace.WriteLine(x.Steps.Count));
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
            private readonly Action<ScenarioSteps<TSpec>> execute;
            private readonly ScenarioSteps<TSpec> steps;
            private Stage stage;

            internal Scenario(Action<ScenarioSteps<TSpec>> execute)
            {
                if (execute == null)
                {
                    throw new ArgumentNullException(nameof(execute));
                }
                this.execute = execute;
                this.stage = Stage.Given;
                this.steps = new ScenarioSteps<TSpec>();
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
                steps.Append(stage, expression);
            }
        }

        [DebuggerDisplay("Steps: {Steps.Count}")]
        internal sealed class ScenarioSteps<TSpec>
        {
            private readonly Dictionary<Stage, List<ScenarioStep<TSpec>>> steps;

            public ScenarioSteps()
            {
                this.steps = new Dictionary<Stage, List<ScenarioStep<TSpec>>>();
                foreach (var stage in EnumX.GetValues<Stage>())
                {
                    steps.Add(stage, new List<ScenarioStep<TSpec>>());
                }
            }
            internal IReadOnlyCollection<ScenarioStep<TSpec>> Steps
                => steps.SelectMany(x => x.Value).ToList().AsReadOnly();

            public void Append(Stage stage, Expression<Action<TSpec>> expression)
            {
                var methodCallExpression = expression.Body as MethodCallExpression;
                if (methodCallExpression == null)
                {
                    throw new NotSupportedException(@"The scenario step should be as a single method call.");
                }
                var stepAttribute = methodCallExpression.Method.GetCustomAttribute<StepAttribute>();
                if (stepAttribute == null)
                {
                    throw new NotSupportedException(
                        @"The spec step should be defined with one of Given, When or Then attribute.");
                }
                if (stepAttribute.Stage != stage)
                {
                    throw new NotSupportedException(@"The scenario step should have the same stage as in spec.");
                }
                steps[stage].Add(new ScenarioStep<TSpec>(expression));
            }
        }

        [DebuggerDisplay("Step: {Title}")]
        internal sealed class ScenarioStep<TSpec>
        {
            public ScenarioStep(Expression<Action<TSpec>> expression)
            {
                Expression = expression;
            }

            public Expression<Action<TSpec>> Expression { get; private set; }
            public string Title { get; }
            public Action Action { get; }
        }
    }

    internal static class EnumX
    {
        public static T Parse<T>(string value) => (T) Enum.Parse(typeof (T), value, true);
        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof (T)).OfType<T>();
    }
}