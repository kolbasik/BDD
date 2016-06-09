using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public class RunnerTracing
    {
        private readonly Logger logger;

        public RunnerTracing(Logger logger)
        {
            this.logger = logger;
        }

        public void FeatureEnter(Feature feature)
        {
            logger.WriteLine("Feature:");
            if (!string.IsNullOrWhiteSpace(feature.AsA))
                logger.WriteLine("\tAs a " + feature.AsA);
            if (!string.IsNullOrWhiteSpace(feature.IWant))
                logger.WriteLine("\tI want " + feature.IWant);
            if (!string.IsNullOrWhiteSpace(feature.SoThat))
                logger.WriteLine("\tSo that " + feature.SoThat);
        }

        public void ScenarioEnter(Scenario scenario)
        {
            logger.WriteLine($"{Environment.NewLine}Scenario:");
        }

        public void StepEnter(Step step)
        {
        }

        public void StepExit(Step step, StepResult stepResult)
        {
            if (stepResult.Name.StartsWith(@"Given", StringComparison.OrdinalIgnoreCase))
            {
                logger.WriteLine(string.Empty);
            }
            logger.WriteLine(string.Format("\t{0} {1} {2}",
                stepResult.Name,
                stepResult.Success.HasValue
                    ? (stepResult.Success.Value ? @"PASSED" : @"FAILED")
                    : string.Empty,
                stepResult.Elapsed.HasValue
                    ? stepResult.Elapsed.Value.TotalMilliseconds.ToString(@"F1") + @"ms"
                    : null));
        }

        public void ScenarioExit(Scenario scenario, ScenarioResult scenarioResult)
        {
            logger.WriteLine($"{Environment.NewLine}{scenarioResult.Exception}");
        }

        public void FeatureExit(Feature feature, FeatureResult featureResult)
        {
        }
    }
}