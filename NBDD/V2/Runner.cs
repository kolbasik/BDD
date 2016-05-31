using System;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class Runner
    {
        public static readonly CompositionContainer Container;

        static Runner()
        {
            Container = new CompositionContainer();
            Container.Register(nameof(Runner), new Tracer(Bdd.Trace));
        }

        [DebuggerHidden]
        public static Feature Feature()
        {
            return new Feature(Container);
        }

        [DebuggerHidden]
        public static async Task TestAsync(this Feature feature)
        {
            var featureResult = await PlayAsync(feature).ConfigureAwait(false);
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

        [DebuggerHidden]
        public static async Task<FeatureResult> PlayAsync(this Feature feature)
        {
            using (feature)
            {
                var stopwatch = Stopwatch.StartNew();
                var logger = feature.Container.ResolveOrDefault<Tracer>() ??
                             Container.Resolve<Tracer>(nameof(Runner));

                logger.Trace("Feature:");
                logger.Trace("\tAs a " + feature.AsA);
                logger.Trace("\tI want " + feature.IWant);
                logger.Trace("\tSo that " + feature.SoThat);

                var featureResult = new FeatureResult(feature);
                foreach (var scenario in feature.Scenarios)
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
                                    scenarioResult.Steps.Add(new StepResult(null, scenario.Transform(step.Title), null));
                                }
                                else
                                {
                                    stopwatch.Restart();
                                    try
                                    {
                                        await unit.Action.Invoke().ConfigureAwait(false);
                                        scenarioResult.Steps.Add(new StepResult(true, scenario.Transform(step.Title),
                                            stopwatch.Elapsed));
                                    }
                                    catch (Exception ex)
                                    {
                                        skip = true;
                                        if (ex is TargetInvocationException)
                                        {
                                            ex = ex.InnerException;
                                        }
                                        scenarioResult.Exception = ex;
                                        scenarioResult.Steps.Add(new StepResult(false, scenario.Transform(step.Title),
                                            stopwatch.Elapsed));
                                    }
                                }
                                var stepResult = scenarioResult.Steps.Last();
                                if (stepResult.Name.StartsWith(@"Given", StringComparison.OrdinalIgnoreCase))
                                {
                                    logger.Trace(string.Empty);
                                }
                                logger.Trace(string.Format("\t{0} {1} {2}",
                                    stepResult.Name,
                                    stepResult.Success.HasValue
                                        ? (stepResult.Success.Value ? @"PASSED" : @"FAILED")
                                        : string.Empty,
                                    stepResult.Elapsed.HasValue
                                        ? stepResult.Elapsed.Value.TotalMilliseconds.ToString(@"F1") + @"ms"
                                        : null));
                            }
                            else
                            {
                                if (skip)
                                {
                                    // NOTE: do nothing
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