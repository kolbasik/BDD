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
            Units = new List<Unit>();
            Components = new List<Component>();
            Container = feature.Container.Scope();
            Props = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            Components.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        internal List<Unit> Units { get; }
        internal List<Component> Components { get; }
        public CompositionContainer Container { get; }
        public Dictionary<string, object> Props { get; }

        // TOSO: moved to extensions
        public Scenario Bind(Action<Scenario> bind)
        {
            return Bind(bind.AsAsync());
        }

        public Scenario Bind(Func<Scenario, Task> bind)
        {
            Units.Add(new Bind(bind.Bind(this)));
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