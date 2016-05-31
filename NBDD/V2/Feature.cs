using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public sealed class Feature : IDisposable
    {
        public Feature(CompositionContainer container)
        {
            Scenarios = new List<Scenario>();
            Container = container.Scope();
        }

        public void Dispose()
        {
            Scenarios.ForEach(x => x.Dispose());
            Container.Dispose();
        }

        public string AsA { get; set; }
        public string IWant { get; set; }
        public string SoThat { get; set; }

        internal List<Scenario> Scenarios { get; }
        public CompositionContainer Container { get; }

        [DebuggerHidden]
        public Scenario Scenario()
        {
            var scenario = new Scenario(this);
            Scenarios.Add(scenario);
            return scenario;
        }

        [DebuggerHidden]
        public Feature Describe(Action<Feature> describe)
        {
            describe(this);
            return this;
        }
    }
}