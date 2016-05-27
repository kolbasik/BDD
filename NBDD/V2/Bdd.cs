using System;
using System.Collections.Generic;
using System.Diagnostics;

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
                    step.Action.Invoke();
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

        public void Step(string title, Action action)
        {
            Steps.Add(new Step(title, action));
        }
    }

    [DebuggerDisplay("Step: {Title}")]
    internal sealed class Step
    {
        public Step(string title, Action action)
        {
            Title = title;
            Action = action;
        }

        public string Title { get; }
        public Action Action { get; }
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

        public Component<TComponent> Given(string title, Action<TComponent> action)
        {
            Step(title, action);
            return this;
        }

        public Component<TComponent> When(string title, Action<TComponent> action)
        {
            Step(title, action);
            return this;
        }

        public Component<TComponent> Then(string title, Action<TComponent> action)
        {
            Step(title, action);
            return this;
        }

        public Component<TComponent> And(string title, Action<TComponent> action)
        {
            Step(title, action);
            return this;
        }

        internal void Step(string title, Action<TComponent> action)
        {
            Scenario.Step(title, () => action(Instance));
        }
    }

    public static class ComponentExtensions
    {
        public static Component<TComponent> Use<TComponent>(this Scenario scenario) where TComponent : class, new()
        {
            return new Component<TComponent>(scenario);
        }

        public static Component<TComponentNew> Use<TComponentOld, TComponentNew>(this Component<TComponentOld> componentOld)
            where TComponentOld : class, new()
            where TComponentNew : class, new()
        {
            return new Component<TComponentNew>(componentOld.Scenario);
        }
    }
}