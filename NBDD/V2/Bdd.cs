using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

    [DebuggerStepThrough, DebuggerNonUserCode]
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

        [DebuggerHidden, DebuggerStepThrough]
        public Feature Describe(Action<Feature> describe)
        {
            describe(this);
            return this;
        }

        [DebuggerHidden, DebuggerStepThrough]
        public FeatureResult Play()
        {
            return PlayAsync().GetAwaiter().GetResult();
        }

        [DebuggerHidden, DebuggerStepThrough]
        public async Task<FeatureResult> PlayAsync()
        {
            Trace.WriteLine("Feature:");
            Trace.WriteLine('\t' + "As a " + AsA);
            Trace.WriteLine('\t' + "I want " + IWant);
            Trace.WriteLine('\t' + "So that " + SoThat);

            var featureResult = new FeatureResult(this);
            foreach (var scenario in Scenarios)
            {
                var skip = false;
                Trace.WriteLine($"{Environment.NewLine}Scenario:");
                var scenarioResult = new ScenarioResult(scenario);
                foreach (var step in scenario.Steps)
                {
                    if (skip)
                    {
                        scenarioResult.Steps.Add(new StepResult(null, step.Title));
                    }
                    else
                    {
                        try
                        {
                            await step.Action.Invoke().ConfigureAwait(false);
                            scenarioResult.Steps.Add(new StepResult(true, step.Title));
                        }
                        catch (Exception ex)
                        {
                            skip = true;
                            if (ex is TargetInvocationException)
                            {
                                ex = ex.InnerException;
                            }
                            scenarioResult.Exception = ex;
                            scenarioResult.Steps.Add(new StepResult(false, step.Title));
                        }
                    }
                    Trace.WriteLine("\t" + scenarioResult.Steps.Last());
                }
                Trace.WriteLine(string.Empty);
                Trace.WriteLineIf(scenarioResult.Exception != null, scenarioResult.Exception);
                featureResult.Scenarios.Add(scenarioResult);
            }
            return featureResult;
        }
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Scenario: steps={Steps.Count}")]
    public class Scenario
    {
        public Scenario()
        {
            Steps = new List<Step>();
        }

        internal List<Step> Steps { get; }

        public Scenario Step(string title, Func<Task> action)
        {
            Steps.Add(new Step(title, action));
            return this;
        }
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
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

    [DebuggerStepThrough, DebuggerNonUserCode]
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
            return Step(@"Given " + title, action);
        }

        public Component<TComponent> When(string title, Func<TComponent, Task> action)
        {
            return Step(@"When " + title, action);
        }

        public Component<TComponent> Then(string title, Func<TComponent, Task> action)
        {
            return Step(@"Then " + title, action);
        }

        public Component<TComponent> And(string title, Func<TComponent, Task> action)
        {
            return Step(@" and " + title, action);
        }

        [DebuggerHidden, DebuggerStepThrough]
        internal Component<TComponent> Step(string title, Func<TComponent, Task> action)
        {
            Scenario.Step(title, () => action(Instance));
            return this;
        }
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
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
            return component.Step(@"Given " + title, action);
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(@"When " + title, action);
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(@"Then " + title, action);
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(@" and " + title, action);
        }

        [DebuggerHidden, DebuggerStepThrough]
        private static Component<TComponent> Step<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(title, (instance =>
            {
                action(instance);
                return Done;
            }));
        }
    }

    [DebuggerDisplay("Feature: {Scenarios.Count}")]
    public sealed class FeatureResult
    {
        public FeatureResult(Feature feature)
        {
            Feature = feature;
            Scenarios = new List<ScenarioResult>();
        }

        public Feature Feature { get; }
        public List<ScenarioResult> Scenarios { get; }
    }

    [DebuggerDisplay("Scenario: {Steps.Count} {Exception}")]
    public sealed class ScenarioResult
    {
        public ScenarioResult(Scenario scenario)
        {
            Scenario = scenario;
            Steps = new List<StepResult>(scenario.Steps.Count);
        }

        public Scenario Scenario { get; }
        public List<StepResult> Steps { get; }
        public Exception Exception { get; set; }
    }

    [DebuggerDisplay("Step: {Success} {Name}")]
    public sealed class StepResult
    {
        public StepResult(bool? success, string name)
        {
            Success = success;
            Name = name;
        }

        public bool? Success { get; }
        public string Name { get; }

        public override string ToString() => Name + (Success.HasValue ? (Success.Value ? @" - PASSED" : @" - FAILED") : string.Empty);
    }
}