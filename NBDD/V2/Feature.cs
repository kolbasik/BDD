using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NBDD.V2
{
    public sealed class Feature : IDisposable
    {
        public Feature()
        {
            Scenarios = new List<Scenario>();
            Container = Bdd.Container.Scope();
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

        public Scenario Scenario()
        {
            var scenario = new Scenario(this);
            Scenarios.Add(scenario);
            return scenario;
        }

        public Feature Describe(Action<Feature> describe)
        {
            describe(this);
            return this;
        }

        public async Task TestAsync()
        {
            var featureResult = await PlayAsync().ConfigureAwait(false);
            var exceptions = featureResult.Scenarios.Select(x => x.Exception).Where(x => x != null).ToArray();
            if (exceptions.Length > 0)
            {
                if (exceptions.Length == 1)
                {
                    ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
                }
                else
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public async Task<FeatureResult> PlayAsync()
        {
            using (this)
            {
                var logger = Container.Resolve<Tracer>();

                logger.Trace("Feature:");
                logger.Trace("\tAs a " + AsA);
                logger.Trace("\tI want " + IWant);
                logger.Trace("\tSo that " + SoThat);

                var featureResult = new FeatureResult(this);
                foreach (var scenario in Scenarios)
                {
                    using (scenario)
                    {
                        var skip = false;
                        logger.Trace($"{Environment.NewLine}Scenario:");
                        var scenarioResult = new ScenarioResult(scenario);
                        foreach (var unit in scenario.Units)
                        {
                            var step = unit as Step;
                            if (step != null)
                            {
                                if (skip)
                                {
                                    scenarioResult.Steps.Add(new StepResult(null, scenario.Transform(step.Title)));
                                }
                                else
                                {
                                    try
                                    {
                                        await unit.Action.Invoke().ConfigureAwait(false);
                                        scenarioResult.Steps.Add(new StepResult(true, scenario.Transform(step.Title)));
                                    }
                                    catch (Exception ex)
                                    {
                                        skip = true;
                                        if (ex is TargetInvocationException)
                                        {
                                            ex = ex.InnerException;
                                        }
                                        scenarioResult.Exception = ex;
                                        scenarioResult.Steps.Add(new StepResult(false, scenario.Transform(step.Title)));
                                    }
                                }
                                var stepResult = scenarioResult.Steps.Last();
                                if (stepResult.Name.StartsWith(@"Given", StringComparison.OrdinalIgnoreCase))
                                {
                                    logger.Trace(string.Empty);
                                }
                                logger.Trace("\t" + stepResult.Name + (stepResult.Success.HasValue ? (stepResult.Success.Value ? @" - PASSED" : @" - FAILED") : string.Empty));
                            }
                            else
                            {
                                try
                                {
                                    await unit.Action.Invoke().ConfigureAwait(false);
                                }
                                catch (Exception ex)
                                {
                                    skip = true;
                                    if (ex is TargetInvocationException)
                                    {
                                        ex = ex.InnerException;
                                    }
                                    scenarioResult.Exception = ex;
                                }
                            }
                        }
                        logger.Trace($"{Environment.NewLine}{scenarioResult.Exception}");
                        featureResult.Scenarios.Add(scenarioResult);
                    }
                }
                return featureResult;
            }
        }
    }
}