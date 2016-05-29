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
    public class Component<TActions> : Component
    {
        public Component(Scenario scenario) : base(scenario)
        {
            Actions = new Lazy<TActions>(Scenario.Container.Resolve<TActions>);
        }

        [DebuggerHidden]
        public override void Dispose()
        {
            if (Actions.IsValueCreated)
            {
                var disposable = Actions.Value as IDisposable;
                disposable?.Dispose();
            }
        }

        internal Lazy<TActions> Actions { get; }

        [DebuggerHidden]
        public Component<TActions> Given(string title, Func<TActions, Scenario, Task> action)
        {
            Scenario.Step(@"Given " + title, action.Bind(Actions).Bind(Scenario));
            return this;
        }

        [DebuggerHidden]
        public Component<TActions> When(string title, Func<TActions, Scenario, Task> action)
        {
            Scenario.Step(@"When " + title, action.Bind(Actions).Bind(Scenario));
            return this;
        }

        [DebuggerHidden]
        public Component<TActions> Then(string title, Func<TActions, Scenario, Task> action)
        {
            Scenario.Step(@"Then " + title, action.Bind(Actions).Bind(Scenario));
            return this;
        }

        [DebuggerHidden]
        public Component<TActions> And(string title, Func<TActions, Scenario, Task> action)
        {
            Scenario.Step(@" and " + title, action.Bind(Actions).Bind(Scenario));
            return this;
        }

        [DebuggerHidden]
        internal Component<TActions> Bind(Func<TActions, Scenario, Task> bind)
        {
            Scenario.Bind(bind.Bind(Actions));
            return this;
        }
    }
}