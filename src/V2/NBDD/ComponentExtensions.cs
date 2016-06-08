using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
        public static Component<TActions> Use<TActions>(this Component component)
        {
            return new Component<TActions>(component.Scenario);
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

        [DebuggerHidden]
        public static Component<TActions> Given<TActions>(this Component<TActions> component, Expression<Action<TActions, Scenario>> expression)
        {
            return component.Given(Humanizer.ToString(expression), expression.Compile().AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> When<TActions>(this Component<TActions> component, Expression<Action<TActions, Scenario>> expression)
        {
            return component.When(Humanizer.ToString(expression), expression.Compile().AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> Then<TActions>(this Component<TActions> component, Expression<Action<TActions, Scenario>> expression)
        {
            return component.Then(Humanizer.ToString(expression), expression.Compile().AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> And<TActions>(this Component<TActions> component, Expression<Action<TActions, Scenario>> expression)
        {
            return component.And(Humanizer.ToString(expression), expression.Compile().AsAsync());
        }

        [DebuggerHidden]
        public static Component<TActions> Given<TActions>(this Component<TActions> component, Expression<Func<TActions, Scenario, Task>> expression)
        {
            return component.Given(Humanizer.ToString(expression), expression.Compile());
        }

        [DebuggerHidden]
        public static Component<TActions> When<TActions>(this Component<TActions> component, Expression<Func<TActions, Scenario, Task>> expression)
        {
            return component.When(Humanizer.ToString(expression), expression.Compile());
        }

        [DebuggerHidden]
        public static Component<TActions> Then<TActions>(this Component<TActions> component, Expression<Func<TActions, Scenario, Task>> expression)
        {
            return component.Then(Humanizer.ToString(expression), expression.Compile());
        }

        [DebuggerHidden]
        public static Component<TActions> And<TActions>(this Component<TActions> component, Expression<Func<TActions, Scenario, Task>> expression)
        {
            return component.And(Humanizer.ToString(expression), expression.Compile());
        }
    }
}