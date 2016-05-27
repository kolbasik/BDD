using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    public static class Bdd
    {
        public static Feature Feature(string skip = null, string AsA = null, string IWant = null, string SoThat = null)
        {
            return new Feature { AsA = AsA, IWant = IWant, SoThat = SoThat };
        }
    }

    public sealed class Feature
    {
        public Feature()
        {
            Scenarios = new List<Scenario>();
        }

        public string AsA { get; set; }
        public string IWant { get; set; }
        public string SoThat { get; set; }

        internal List<Scenario> Scenarios { get; }

        public Scenario Scenario()
        {
            var scenario = new Scenario();
            Scenarios.Add(scenario);
            return scenario;
        }

        public Feature Describe(Action<Feature> describe)
        {
            describe(this);
            return this;
        }

        public void Play()
        {
            PlayAsync().GetAwaiter().GetResult();
        }

        public async Task PlayAsync()
        {
            Trace.WriteLine("Feature:");
            Trace.WriteLine('\t' + "As a " + AsA);
            Trace.WriteLine('\t' + "I want " + IWant);
            Trace.WriteLine('\t' + "So that " + SoThat);

            foreach (var scenario in Scenarios)
            {
                Trace.WriteLine($"{Environment.NewLine}Scenario:");
                foreach (var step in scenario.Steps)
                {
                    Trace.WriteLine('\t' + step.Title);
                    await step.Action.Invoke().ConfigureAwait(false);
                }
            }
        }
    }

    [DebuggerDisplay("Steps: {Steps.Count}")]
    public class Scenario
    {
        public Scenario()
        {
            Steps = new List<Step>();
        }

        internal List<Step> Steps { get; }

        public void Step(string title, Func<Task> action)
        {
            Steps.Add(new Step(title, action));
        }
    }

    [DebuggerDisplay("Step: {Title}")]
    internal sealed class Step
    {
        public Step(string title, Func<Task> action)
        {
            Title = title;
            Action = action;
        }

        public string Title { get; }
        public Func<Task> Action { get; }
    }

    public class Component<TComponent> where TComponent : class, new()
    {
        public Component(Scenario scenario)
        {
            Scenario = scenario;
            Instance = new TComponent();
        }

        internal Scenario Scenario { get; }
        internal TComponent Instance { get; }

        public Component<TComponent> Given(string title, Func<TComponent, Task> action)
        {
            return Step(title, action);
        }

        public Component<TComponent> When(string title, Func<TComponent, Task> action)
        {
            return Step(title, action);
        }

        public Component<TComponent> Then(string title, Func<TComponent, Task> action)
        {
            return Step(title, action);
        }

        public Component<TComponent> And(string title, Func<TComponent, Task> action)
        {
            return Step(title, action);
        }

        internal Component<TComponent> Step(string title, Func<TComponent, Task> action)
        {
            Scenario.Step(title, () => action(Instance));
            return this;
        }
    }

    public static class ComponentExtensions
    {
        private static readonly Task Done = Task.FromResult(true);

        public static Component<TComponent> Use<TComponent>(this Scenario scenario)
            where TComponent : class, new()
        {
            return new Component<TComponent>(scenario);
        }

        public static Component<TComponentNew> Use<TComponentOld, TComponentNew>(this Component<TComponentOld> componentOld)
            where TComponentOld : class, new()
            where TComponentNew : class, new()
        {
            return new Component<TComponentNew>(componentOld.Scenario);
        }

        public static Component<TComponent> Given<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, action);
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, action);
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, action);
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, action);
        }

        private static Component<TComponent> Step<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, instance =>
            {
                action(instance);
                return Done;
            });
        }
    }
}