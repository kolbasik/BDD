using System;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class FeatureExtensions
    {
        [DebuggerHidden]
        public static Feature UseBdd<TService>(this Feature feature)
        {
            var service = feature.Container.Resolve<TService>(nameof(Bdd));
            return feature.Use(service);
        }

        [DebuggerHidden]
        public static Feature UseTrace(this Feature feature, Action<string> trace)
        {
            return feature.Use(new Logger(trace));
        }

        [DebuggerHidden]
        public static Feature Use<TService>(this Feature feature, TService service)
        {
            feature.Container.Register<TService>(service);
            return feature;
        }
    }
}