using System;

namespace NBDD.V2
{
    public static class ComponentExtensions
    {
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

        public static Component<TComponent> Bind<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(StepType.Given, title, action);
        }

        public static Component<TComponent> Given<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(StepType.Given, title, action);
        }

        public static Component<TComponent> When<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(StepType.When, title, action);
        }

        public static Component<TComponent> Then<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(StepType.Then, title, action);
        }

        public static Component<TComponent> And<TComponent>(this Component<TComponent> component, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(StepType.And, title, action);
        }

        private static Component<TComponent> Step<TComponent>(this Component<TComponent> component, StepType stepType, string title, Action<TComponent> action)
            where TComponent : class
        {
            return component.Step(stepType, title, instance =>
            {
                action(instance);
                return Bdd.Done;
            });
        }
    }
}