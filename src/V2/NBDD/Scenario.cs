using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    [DebuggerDisplay("Scenario: steps={Steps.Count}")]
    public class Scenario : IDisposable
    {
        public Scenario(Feature feature)
        {
            Steps = new List<Step>();
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

        internal List<Step> Steps { get; }
        internal List<Component> Components { get; }
        public DI Container { get; }
        public Dictionary<string, object> Props { get; }

        [DebuggerHidden]
        public Scenario Bind(Func<Task> action)
        {
            Steps.Add(new Step(string.Empty, action));
            return this;
        }

        [DebuggerHidden]
        public Scenario Step(string title, Func<Task> action)
        {
            Steps.Add(new Step(title, action));
            return this;
        }
    }
}