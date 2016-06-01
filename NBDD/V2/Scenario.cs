using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
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

        [DebuggerHidden]
        public void Dispose()
        {
            Components.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        internal List<Unit> Units { get; }
        internal List<Component> Components { get; }
        public DI Container { get; }
        public Dictionary<string, object> Props { get; }

        [DebuggerHidden]
        public Scenario Bind(Func<Task> bind)
        {
            Units.Add(new Bind(bind));
            return this;
        }

        [DebuggerHidden]
        public Scenario Step(string title, Func<Task> action)
        {
            Units.Add(new Step(title, action));
            return this;
        }
    }
}