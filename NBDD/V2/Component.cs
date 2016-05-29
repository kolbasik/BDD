using System;
using System.Threading.Tasks;

namespace NBDD.V2
{
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

    public class Component<TComponent> : Component where TComponent : class
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

        public Component<TComponent> Given(string title, Func<Scenario, TComponent, Task> action)
        {
            return Step(StepType.Given, title, component => action(Scenario, component));
        }

        public Component<TComponent> When(string title, Func<TComponent, Task> action)
        {
            return Step(StepType.When, title, action);
        }

        public Component<TComponent> Then(string title, Func<TComponent, Task> action)
        {
            return Step(StepType.Then, title, action);
        }

        public Component<TComponent> And(string title, Func<TComponent, Task> action)
        {
            return Step(StepType.And, title, action);
        }

        internal Component<TComponent> Step(StepType stepType, string title, Func<TComponent, Task> action)
        {
            Scenario.Step(stepType, title, () => action(Instance.Value));
            return this;
        }
    }
}