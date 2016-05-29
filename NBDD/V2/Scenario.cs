using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerDisplay("Scenario: units={Units.Count}")]
    public class Scenario : IDisposable
    {
        public Scenario(Feature feature)
        {
            Props = new Dictionary<string, object>();
            Units = new List<Unit>();
            Components = new List<Component>();
            Container = feature.Container.Scope();
        }

        public void Dispose()
        {
            Components.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        internal Dictionary<string, object> Props { get; }
        internal List<Unit> Units { get; }
        internal List<Component> Components { get; }
        public CompositionContainer Container { get; }

        public Scenario Bind(string name, Func<Scenario, object> bind)
        {
            return Bind(s =>
            {
                Props[name] = bind.Invoke(this);
                return Bdd.Done;
            });
        }

        public Scenario Bind(Func<Scenario, Task> bind)
        {
            Units.Add(new Bind(() => bind.Invoke(this)));
            return this;
        }

        public Scenario Step(StepType stepType, string title, Func<Task> action)
        {
            const string space = @" ";
            var display = stepType.ToString();
            if (stepType == StepType.And)
            {
                display = space + display.ToLower();
            }
            display += space + title;
            Units.Add(new Step(display, action));
            return this;
        }
    }
}