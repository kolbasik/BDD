using System;
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
        static Runner()
        {
            DI.Global.Register(nameof(Runner), new Logger(Bdd.Trace));
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
            var tracing = new RunnerTracing(Resolve<Logger>(feature.Container));
            tracing.FeatureEnter(feature);
            using (feature)
            {
                var featureResult = new FeatureResult(feature);
                foreach (var scenario in feature.Scenarios)
                {
                    tracing.ScenarioEnter(scenario);
                    using (scenario)
                    {
                        var skip = false;
                        var scenarioResult = new ScenarioResult(scenario);
                        foreach (var step in scenario.Steps)
                        {
                            if (!string.IsNullOrEmpty(step.Title))
                            {
                                tracing.StepEnter(step);
                                if (skip)
                                {
                                    scenarioResult.Steps.Add(StepResult.Skip(scenario.Transform(step.Title)));
                                }
                                else
                                {
                                    var stopwatch = Stopwatch.StartNew();
                                    try
                                    {
                                        await step.Action.Invoke().ConfigureAwait(false);
                                        scenarioResult.Steps.Add(StepResult.Done(scenario.Transform(step.Title), stopwatch.Elapsed));
                                    }
                                    catch (Exception ex)
                                    {
                                        skip = true;
                                        scenarioResult.Exception = HandleException(ex);
                                        scenarioResult.Steps.Add(StepResult.Fail(scenario.Transform(step.Title), stopwatch.Elapsed));
                                    }
                                }
                                var stepResult = scenarioResult.Steps.Last();
                                tracing.StepExit(step, stepResult);
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
                                        await step.Action.Invoke().ConfigureAwait(false);
                                    }
                                    catch (Exception ex)
                                    {
                                        skip = true;
                                        scenarioResult.Exception = HandleException(ex);
                                    }
                                }
                            }
                        }
                        tracing.ScenarioExit(scenario, scenarioResult);
                        featureResult.Scenarios.Add(scenarioResult);
                    }
                }
                tracing.FeatureExit(feature, featureResult);
                return featureResult;
            }
        }

        private static T Resolve<T>(DI container) where T : class
        {
            return container.ResolveOrDefault<T>() ?? DI.Global.Resolve<T>(nameof(Runner));
        }

        private static Exception HandleException(Exception ex)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            return ex;
        }
    }
}