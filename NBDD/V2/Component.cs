using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public abstract class Component : IDisposable
    {
        public Component(Scenario scenario)
        {
            Scenario = scenario;
            Scenario.Components.Add(this);
        }

        internal Scenario Scenario { get; }

        [DebuggerHidden]
        public abstract void Dispose();
    }

    [DebuggerStepThrough, DebuggerNonUserCode]
    public class Component<TComponent> : Component
    {
        public Component(Scenario scenario) : base(scenario)
        {
            Instance = new Lazy<TComponent>(Scenario.Container.Resolve<TComponent>);
        }

        [DebuggerHidden]
        public override void Dispose()
        {
            if (Instance.IsValueCreated)
            {
                var disposable = Instance.Value as IDisposable;
                disposable?.Dispose();
            }
        }

        internal Lazy<TComponent> Instance { get; }

        [DebuggerHidden]
        public Component<TComponent> Given(string title, Func<TComponent, Scenario, Task> action)
        {
            return Step(StepType.Given, title, action);
        }

        [DebuggerHidden]
        public Component<TComponent> When(string title, Func<TComponent, Scenario, Task> action)
        {
            return Step(StepType.When, title, action);
        }

        [DebuggerHidden]
        public Component<TComponent> Then(string title, Func<TComponent, Scenario, Task> action)
        {
            return Step(StepType.Then, title, action);
        }

        [DebuggerHidden]
        public Component<TComponent> And(string title, Func<TComponent, Scenario, Task> action)
        {
            return Step(StepType.And, title, action);
        }

        [DebuggerHidden]
        internal Component<TComponent> Step(StepType stepType, string title, Func<TComponent, Scenario, Task> action)
        {
            Scenario.Step(stepType, title, action.Bind(Instance).Bind(Scenario));
            return this;
        }

        [DebuggerHidden]
        internal Component<TComponent> Bind(Func<TComponent, Scenario, Task> bind)
        {
            Scenario.Bind(bind.Bind(Instance));
            return this;
        }
    }
}