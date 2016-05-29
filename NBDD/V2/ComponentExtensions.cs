using System;

namespace NBDD.V2
{
    public static class ComponentExtensions
    {
        public static Component<TComponent> Use<TComponent>(this Scenario scenario)
        {
            return new Component<TComponent>(scenario);
        }

        public static Component<TComponent> Use<TComponent>(this Component componentOld)
        {
            return new Component<TComponent>(componentOld.Scenario);
        }

        public static Component<TComponent> Given<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.Given(title, action.AsAsync());
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.When(title, action.AsAsync());
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.Then(title, action.AsAsync());
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent, Scenario> action)
        {
            return component.And(title, action.AsAsync());
        }

        public static Component<TComponent> Bind<TComponent>(this Component<TComponent> component, Action<TComponent, Scenario> action)
        {
            return component.Bind(action.AsAsync());
        }
    }
}