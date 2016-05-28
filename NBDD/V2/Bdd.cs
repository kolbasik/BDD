using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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

    public enum Stage
    {
        And,
        Given,
        When,
        Then
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
    public sealed class Feature : IDisposable
    {
        public Feature()
        {
            Scenarios = new List<Scenario>();
            Container = new CompositionContainer(new ApplicationCatalog());
        }

        public void Dispose()
        {
            Scenarios.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        public string AsA { get; set; }
        public string IWant { get; set; }
        public string SoThat { get; set; }

        internal List<Scenario> Scenarios { get; }
        public CompositionContainer Container { get; }

        public Scenario Scenario()
        {
            var scenario = new Scenario(this);
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
        public FeatureResult Play(Action<string> trace)
        {
            return PlayAsync(trace).GetAwaiter().GetResult();
        }

        [DebuggerHidden, DebuggerStepThrough]
        public async Task<FeatureResult> PlayAsync(Action<string> trace)
        {
            trace("Feature:");
            trace("\tAs a " + AsA);
            trace("\tI want " + IWant);
            trace("\tSo that " + SoThat);

            using (this)
            {
                var featureResult = new FeatureResult(this);
                foreach (var scenario in Scenarios)
                {
                    using (scenario)
                    {
                        var skip = false;
                        trace($"{Environment.NewLine}Scenario:");
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
                            var stepResult = scenarioResult.Steps.Last();
                            trace("\t" + stepResult.Name + (stepResult.Success.HasValue ? (stepResult.Success.Value ? @" - PASSED" : @" - FAILED") : string.Empty));
                        }
                        trace($"{Environment.NewLine}{scenarioResult.Exception}");
                        featureResult.Scenarios.Add(scenarioResult);
                    }
                }
                return featureResult;
            }
        }
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Scenario: steps={Steps.Count}")]
    public class Scenario : IDisposable
    {
        public Scenario(Feature feature)
        {
            Steps = new List<Step>();
            Container = new CompositionContainer(feature.Container);
            Components = new List<IDisposable>();
        }

        public void Dispose()
        {
            Components.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        internal List<Step> Steps { get; }
        public CompositionContainer Container { get; }
        internal List<IDisposable> Components { get; }

        public Scenario Step(Stage stage, string title, Func<Task> action)
        {
            const string space = @" ";
            var display = stage.ToString();
            if (stage == Stage.And)
            {
                display = space + display.ToLower();
            }
            display += space + title;
            Steps.Add(new Step(display, action));
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
    public class Component<TComponent> : IDisposable where TComponent : class, new()
    {
        public Component(Scenario scenario)
        {
            Scenario = scenario;
            Instance = new TComponent();
            Scenario.Container.ComposeParts(Instance);
            Scenario.Components.Add(this);
        }

        public void Dispose()
        {
            var disposable = Instance as IDisposable;
            disposable?.Dispose();
        }

        internal Scenario Scenario { get; }
        internal TComponent Instance { get; }

        public Component<TComponent> Given(string title, Func<TComponent, Task> action)
        {
            return Step(Stage.Given, title, action);
        }

        public Component<TComponent> When(string title, Func<TComponent, Task> action)
        {
            return Step(Stage.When, title, action);
        }

        public Component<TComponent> Then(string title, Func<TComponent, Task> action)
        {
            return Step(Stage.Then, title, action);
        }

        public Component<TComponent> And(string title, Func<TComponent, Task> action)
        {
            return Step(Stage.And, title, action);
        }

        [DebuggerHidden, DebuggerStepThrough]
        internal Component<TComponent> Step(Stage stage, string title, Func<TComponent, Task> action)
        {
            Scenario.Step(stage, title, () => action(Instance));
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
            return component.Step(Stage.Given, title, action);
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(Stage.When, title, action);
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(Stage.Then, title, action);
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(Stage.And, title, action);
        }

        [DebuggerHidden, DebuggerStepThrough]
        private static Component<TComponent> Step<TComponent>(this Component<TComponent> component, Stage stage, string title, Action<TComponent> action)
            where TComponent : class, new()
        {
            return component.Step(stage, title, instance =>
            {
                action(instance);
                return Done;
            });
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
    }
}