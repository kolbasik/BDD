using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class ComponentExtensions
    {
        [DebuggerHidden]
        public static Component<TComponent> Use<TComponent>(this Scenario scenario)
        {
            return new Component<TComponent>(scenario);
        }

        [DebuggerHidden]
        public static Component<TComponent> Use<TComponent>(this Component componentOld)
        {
            return new Component<TComponent>(componentOld.Scenario);
        }

        [DebuggerHidden]
        public static Component<TComponent> Given<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.Given(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.When(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.Then(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.And(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TComponent> Bind<TComponent>(this Component<TComponent> component, Action<TComponent, Scenario> action)
        {
            return component.Bind(action.AsAsync());
        }
    }
}