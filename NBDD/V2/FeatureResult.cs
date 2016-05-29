using System.Collections.Generic;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerDisplay("Feature: {Scenarios.Count}")]
    public sealed class FeatureResult
    {
        public FeatureResult(Feature feature)
        {
            Feature = feature;
            Scenarios = new List<ScenarioResult>();
        }

        public Feature Feature { get; }
        public List<ScenarioResult> Scenarios { get; }
    }
}