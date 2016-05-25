using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NBDD
{
    public static class Bdd
    {
        public static Action<SpecResult> Trace
        {
            get { return (Action<SpecResult>) CallContext.LogicalGetData("Bdd.Trace") ?? DefaultTrace; }
            set { CallContext.LogicalSetData("Bdd.Trace", value); }
        }

        private static void DefaultTrace(SpecResult x)
        {
            System.Diagnostics.Trace.WriteLine(x);
        }

        [DebuggerHidden, DebuggerStepThrough]
        public static TSpec DependsOn<TSpec>() where TSpec : new()
        {
            return DependsOnAsync<TSpec>().GetAwaiter().GetResult();
        }

        [DebuggerHidden, DebuggerStepThrough]
        public static async Task<TSpec> DependsOnAsync<TSpec>() where TSpec : new()
        {
            var spec = new TSpec();
            try
            {
                await RunSpecAsync(spec).ConfigureAwait(false);
            }
            finally
            {
                var disposable = spec as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            return spec;
        }

        [DebuggerHidden, DebuggerStepThrough]
        public static void RunSpec<TSpec>(TSpec spec)
        {
            RunSpecAsync(spec).GetAwaiter().GetResult();
        }

        [DebuggerHidden, DebuggerStepThrough]
        public static async Task RunSpecAsync<TSpec>(TSpec spec)
        {
            var result = await SpecRunner.RunAsync(spec).ConfigureAwait(false);
            Trace(result);
            if (result.Exception != null)
            {
                throw new SpecException(result.Meta.Feature, result.Exception);
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class GivenAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class WhenAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method)]
        public sealed class ThenAttribute : Attribute
        {
        }

        private static class SpecRunner
        {
            [DebuggerHidden, DebuggerStepThrough]
            public static async Task<SpecResult> RunAsync<TSpec>(TSpec spec)
            {
                await Task.Yield();
                var result = new SpecResult(SpecMeta.For<TSpec>());
                var queue = new Queue<SpecStepMeta>(result.Meta.Steps);
                try
                {
                    var objects = new object[0];
                    while (queue.Count > 0)
                    {
                        var step = queue.Dequeue();
                        try
                        {
                            var task = step.Method.Invoke(spec, objects) as Task;
                            if (task != null)
                            {
                                await task.ConfigureAwait(false);
                            }
                            result.Steps.Add(new SpecStepResult(true, step.Name));
                        }
                        catch (Exception ex)
                        {
                            if (ex is TargetInvocationException)
                            {
                                ex = ex.InnerException;
                            }
                            result.Steps.Add(new SpecStepResult(false, step.Name));
                            result.Exception = ex;
                            break;
                        }
                    }
                }
                finally
                {
                    while (queue.Count > 0)
                    {
                        var step = queue.Dequeue();
                        result.Steps.Add(new SpecStepResult(null, step.Name));
                    }
                }
                return result;
            }
        }

        public sealed class SpecException : Exception
        {
            internal SpecException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        [DebuggerDisplay("{Feature}")]
        public sealed class SpecMeta
        {
            private SpecMeta(string feature, SpecStepMeta[] steps)
            {
                Feature = feature;
                Steps = steps;
                Scenario = string.Join(Environment.NewLine, Steps.Select(x => x.Name));
            }

            public string Feature { get; private set; }
            public string Scenario { get; private set; }
            public SpecStepMeta[] Steps { get; private set; }

            internal static SpecMeta For<TSpec>()
            {
                return SpecMetaHolder<TSpec>.Instance;
            }

            private static class SpecMetaHolder<TSpec>
            {
                public static readonly SpecMeta Instance;

                static SpecMetaHolder()
                {
                    var feature = typeof (TSpec).Name;
                    var methods =
                        typeof (TSpec).GetMethods(
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
                    var steps =
                        methods.Where(x => x.GetCustomAttributes<GivenAttribute>().Any())
                            .Concat(methods.Where(x => x.GetCustomAttributes<WhenAttribute>().Any()))
                            .Concat(methods.Where(x => x.GetCustomAttributes<ThenAttribute>().Any()))
                            .Select(x => new SpecStepMeta(x.Name.Replace('_', ' '), x))
                            .ToArray();
                    Instance = new SpecMeta(feature, steps);
                }
            }
        }

        [DebuggerDisplay("{Name}")]
        public sealed class SpecStepMeta
        {
            public SpecStepMeta(string name, MethodInfo method)
            {
                Name = name;
                Method = method;
            }

            public string Name { get; private set; }
            public MethodInfo Method { get; private set; }
        }

        [DebuggerDisplay("{Meta}")]
        public sealed class SpecResult
        {
            public SpecResult(SpecMeta meta)
            {
                Meta = meta;
                Steps = new List<SpecStepResult>(meta.Steps.Length);
            }

            public SpecMeta Meta { get; private set; }
            public List<SpecStepResult> Steps { get; private set; }
            public Exception Exception { get; set; }

            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.AppendFormat(@"Feature: " + Meta.Feature).AppendLine();
                sb.AppendFormat(@"Scenario:").AppendLine();
                foreach (var step in Steps)
                {
                    sb.AppendFormat(
                        "[{0}] : {1}",
                        step.Success.HasValue ? (step.Success.Value ? @"+" : @"X") : @" ",
                        step.Name);
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }

        [DebuggerDisplay("{Success} : {Name}")]
        public sealed class SpecStepResult
        {
            public bool? Success { get; private set; }
            public string Name { get; private set; }

            public SpecStepResult(bool? success, string name)
            {
                Success = success;
                Name = name;
            }
        }
    }
}