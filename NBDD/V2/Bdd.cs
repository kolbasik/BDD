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
        public static Scenario<TSpec> Scen<TSpec>() where TSpec : new()
        {
            return Scen(new TSpec());
        }

        public static Scenario<TSpec> Scen<TSpec>(TSpec spec)
        {
            return new Scenario<TSpec>(x => Trace.Write(x.Steps.Count.ToString()));
        }

        public enum Stage
        {
            Given,
            When,
            Then
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public abstract class StepAttribute : Attribute
        {
            protected StepAttribute(Stage stage)
            {
                Stage = stage;
            }

            public Stage Stage { get; private set; }
            public string Title { get; set; }
        }

        public sealed class GivenAttribute : StepAttribute
        {
            public GivenAttribute() : base(Stage.Given)
            {
            }
        }

        public sealed class WhenAttribute : StepAttribute
        {
            public WhenAttribute() : base(Stage.When)
            {
            }
        }

        public sealed class ThenAttribute : StepAttribute
        {
            public ThenAttribute() : base(Stage.Then)
            {
            }
        }

        public sealed class Scenario<TSpec>
        {
            private readonly Action<Scenario<TSpec>> _execute;
            private readonly Dictionary<Stage, List<Expression<Action<TSpec>>>> _steps;
            private Stage _stage;

            internal Scenario(Action<Scenario<TSpec>> execute)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
                _execute = execute;
                _stage = Stage.Given;
                _steps = new Dictionary<Stage, List<Expression<Action<TSpec>>>>();
                foreach (var stage in Enum.GetValues(typeof (Stage)).Cast<Stage>())
                {
                    _steps.Add(stage, new List<Expression<Action<TSpec>>>());
                }
            }

            internal IReadOnlyCollection<Expression<Action<TSpec>>> Steps
                => _steps.SelectMany(x => x.Value).ToList().AsReadOnly();

            public Scenario<TSpec> Given(Expression<Action<TSpec>> expression)
            {
                Append(Stage.Given, expression);
                return this;
            }

            public Scenario<TSpec> When(Expression<Action<TSpec>> expression)
            {
                if (_stage == Stage.Given) _stage = Stage.When;
                Append(Stage.When, expression);
                return this;
            }

            public Scenario<TSpec> Then(Expression<Action<TSpec>> expression)
            {
                if (_stage == Stage.When) _stage = Stage.Then;
                Append(Stage.Then, expression);
                return this;
            }

            public Scenario<TSpec> And(Expression<Action<TSpec>> expression)
            {
                Append(_stage, expression);
                return this;
            }

            public void Execute()
            {
                _execute(this);
            }

            private void Append(Stage stage, Expression<Action<TSpec>> expression)
            {
                if (_stage != stage)
                {
                    throw new NotSupportedException(
                        @"The scenario steps should be ordered in the following order: Given, When, Then.");
                }
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
                _steps[stage].Add(expression);
            }
        }
    }
}