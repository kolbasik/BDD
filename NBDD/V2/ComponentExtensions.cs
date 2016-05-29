using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class ComponentExtensions
    {
        [DebuggerHidden]
        public static Component<TActions> Use<TActions>(this Scenario scenario)
        {
            return new Component<TActions>(scenario);
        }

        [DebuggerHidden]
        public static Component<TActions> Use<TActions>(this Component componentOld)
        {
            return new Component<TActions>(componentOld.Scenario);
        }

        [DebuggerHidden]
        public static Component<TActions> Given<TActions>(this Component<TActions> component, string title, Action<TActions, Scenario> action)
        {
            return component.Given(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> When<TActions>(this Component<TActions> component, string title, Action<TActions, Scenario> action)
        {
            return component.When(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> Then<TActions>(this Component<TActions> component, string title, Action<TActions, Scenario> action)
        {
            return component.Then(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> And<TActions>(this Component<TActions> component, string title, Action<TActions, Scenario> action)
        {
            return component.And(title, action.AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> Bind<TActions>(this Component<TActions> component, Action<TActions, Scenario> action)
        {
            return component.Bind(action.AsAsync());
        }
    }
}