using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace NBDD.V2
{
    public static class Bdd
    {
        public static readonly CompositionContainer Container;

        static Bdd()
        {
            Container = new CompositionContainer().Scope();
            Container.Register(nameof(Bdd), new Tracer());
        }

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

    public sealed class Feature : IDisposable
    {
        public Feature()
        {
            Scenarios = new List<Scenario>();
            Container = Bdd.Container.Scope();
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

        public Feature Describe(Action<Feature> describe)
        {
            describe(this);
            return this;
        }

        public async Task TestAsync()
        {
            var featureResult = await PlayAsync().ConfigureAwait(false);
            var exceptions = featureResult.Scenarios.Select(x => x.Exception).Where(x => x != null).ToArray();
            if (exceptions.Length > 0)
            {
                if (exceptions.Length == 1)
                {
                    ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
                }
                else
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public async Task<FeatureResult> PlayAsync()
        {
            using (this)
            {
                var logger = Container.Resolve<Tracer>();

                logger.Trace("Feature:");
                logger.Trace("\tAs a " + AsA);
                logger.Trace("\tI want " + IWant);
                logger.Trace("\tSo that " + SoThat);

                var featureResult = new FeatureResult(this);
                foreach (var scenario in Scenarios)
                {
                    using (scenario)
                    {
                        var skip = false;
                        logger.Trace($"{Environment.NewLine}Scenario:");
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
                            logger.Trace("\t" + stepResult.Name + (stepResult.Success.HasValue ? (stepResult.Success.Value ? @" - PASSED" : @" - FAILED") : string.Empty));
                        }
                        logger.Trace($"{Environment.NewLine}{scenarioResult.Exception}");
                        featureResult.Scenarios.Add(scenarioResult);
                    }
                }
                return featureResult;
            }
        }
    }

    [DebuggerDisplay("Scenario: steps={Steps.Count}")]
    public class Scenario : IDisposable
    {
        public Scenario(Feature feature)
        {
            Steps = new List<Step>();
            Components = new List<Component>();
            Container = feature.Container.Scope();
        }

        public void Dispose()
        {
            Components.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        internal List<Step> Steps { get; }
        internal List<Component> Components { get; }
        public CompositionContainer Container { get; }

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

    public abstract class Component : IDisposable
    {
        public Component(Scenario scenario)
        {
            Scenario = scenario;
            Scenario.Components.Add(this);
        }

        internal Scenario Scenario { get; }

        public abstract void Dispose();
    }

    public class Component<TComponent> : Component where TComponent :class
    {
        public Component(Scenario scenario) : base(scenario)
        {
            Instance = new Lazy<TComponent>(Scenario.Container.Resolve<TComponent>);
        }

        public override void Dispose()
        {
            if (Instance.IsValueCreated)
            {
                var disposable = Instance.Value as IDisposable;
                disposable?.Dispose();
            }
        }

        internal Lazy<TComponent> Instance { get; }

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

        internal Component<TComponent> Step(Stage stage, string title, Func<TComponent, Task> action)
        {
            Scenario.Step(stage, title, () => action(Instance.Value));
            return this;
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

    public sealed class Tracer
    {
        private readonly Action<string> _trace;

        public Tracer() : this(WriteLine)
        {
        }

        public Tracer(Action<string> trace)
        {
            _trace = trace;
        }

        public void Trace(string message) => _trace.Invoke(message);

        public static void WriteLine(string message) => System.Diagnostics.Trace.WriteLine(message);
    }

    public static class CompositionContainerExtensions
    {
        public static CompositionContainer Register<TService>(this CompositionContainer container, string contractName, TService service)
        {
            container.ComposeExportedValue<TService>(contractName, service);
            return container;
        }

        public static CompositionContainer Register<TService>(this CompositionContainer container, TService service)
        {
            container.ComposeExportedValue<TService>(service);
            return container;
        }

        public static TService Resolve<TService>(this CompositionContainer container, string contractName)
        {
            return container.GetExportedValue<TService>(contractName);
        }

        public static TService Resolve<TService>(this CompositionContainer container) where TService : class
        {
            return container.GetExportedValueOrDefault<TService>() ?? container.Resolve<TService>(nameof(Bdd));
        }

        public static CompositionContainer Scope(this CompositionContainer parent)
        {
            var container = new CompositionContainer(new ApplicationCatalog(), parent);
            return container.Register(container);
        }
    }

    public static class FeatureExtensions
    {
        public static Feature UseBdd<TService>(this Feature feature)
        {
            var service = feature.Container.Resolve<TService>(nameof(Bdd));
            return feature.Use(service);
        }

        public static Feature UseTrace(this Feature feature, Action<string> trace)
        {
            return feature.Use(new Tracer(trace));
        }

        public static Feature Use<TService>(this Feature feature, TService service)
        {
            feature.Container.Register<TService>(service);
            return feature;
        }
    }

    public static class ComponentExtensions
    {
        private static readonly Task Done = Task.FromResult(true);

        public static Component<TComponent> Use<TComponent>(this Scenario scenario)
            where TComponent : class
        {
            return new Component<TComponent>(scenario);
        }

        public static Component<TComponent> Use<TComponent>(this Component componentOld)
            where TComponent : class
        {
            return new Component<TComponent>(componentOld.Scenario);
        }

        public static Component<TComponent> Given<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(Stage.Given, title, action);
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(Stage.When, title, action);
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(Stage.Then, title, action);
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(Stage.And, title, action);
        }

        private static Component<TComponent> Step<TComponent>(this Component<TComponent> component, Stage stage, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(stage, title, instance =>
            {
                action(instance);
                return Done;
            });
        }
    }
}