using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NBDD.V2
{
    public static class Bdd
    {
        public static FeatureSpec Feature(string skip = null, string AsA = null, string IWant = null, string SoThat = null)
        {
            return new FeatureSpec(AsA, IWant, SoThat);
        }

        public sealed class FeatureSpec
        {
            public FeatureSpec(string asA, string iWant, string soThat)
            {
                AsA = asA;
                IWant = iWant;
                SoThat = soThat;
                Scenarios = new List<ScenarioSpec>();
            }

            public string AsA { get; }
            public string IWant { get; }
            public string SoThat { get; }

            internal List<ScenarioSpec> Scenarios { get; }

            public ScenarioSpec<TSpec> Scenario<TSpec>() where TSpec : class, new()
            {
                return Scenario(new TSpec());
            }

            public ScenarioSpec<TSpec> Scenario<TSpec>(TSpec spec) where TSpec : class
            {
                var scenario = new ScenarioSpec<TSpec>(spec);
                Scenarios.Add(scenario);
                return scenario;
            }

            public void Execute(Action<FeatureSpec> configure)
            {
                Trace.WriteLine("Feature:");
                Trace.WriteLine('\t' + "As a " + AsA);
                Trace.WriteLine('\t' + "I want " + IWant);
                Trace.WriteLine('\t' + "So that " + SoThat);

                configure(this);

                foreach (var scenario in Scenarios)
                {
                    Trace.WriteLine($"{Environment.NewLine}Scenario:");
                    foreach (var step in scenario.Steps)
                    {
                        Trace.WriteLine('\t' + step.Title);
                        step.Action.Invoke();
                    }
                }
            }
        }

        [DebuggerDisplay("Steps: {steps.Count}")]
        public class ScenarioSpec
        {
            protected ScenarioSpec()
            {
                this.Steps = new List<ScenarioStep>();
            }

            internal List<ScenarioStep> Steps { get; }

            protected void Step(string title, Action action)
            {
                this.Steps.Add(new ScenarioStep(title, action));
            }
        }

        public class ScenarioSpec<TSpec> : ScenarioSpec where TSpec : class
        {
            private readonly TSpec spec;

            public ScenarioSpec(TSpec spec)
            {
                if (spec == null)
                {
                    throw new ArgumentNullException(nameof(spec));
                }
                this.spec = spec;
            }

            public ScenarioSpec<TSpec> Given(string title, Action<TSpec> action)
            {
                Step(title, action);
                return this;
            }

            public ScenarioSpec<TSpec> When(string title, Action<TSpec> action)
            {
                Step(title, action);
                return this;
            }

            public ScenarioSpec<TSpec> Then(string title, Action<TSpec> action)
            {
                Step(title, action);
                return this;
            }

            public ScenarioSpec<TSpec> And(string title, Action<TSpec> action)
            {
                Step(title, action);
                return this;
            }

            protected void Step(string title, Action<TSpec> action)
            {
                Step(title, () => action(this.spec));
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
    }
}