using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerDisplay("Scenario: {Steps.Count} {Exception}")]
    public sealed class ScenarioResult
    {
        public ScenarioResult(Scenario scenario)
        {
            Scenario = scenario;
            Steps = new List<StepResult>();
        }

        public Scenario Scenario { get; }
        public List<StepResult> Steps { get; }
        public Exception Exception { get; set; }
    }
}