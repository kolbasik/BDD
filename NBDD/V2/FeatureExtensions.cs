using System;

namespace NBDD.V2
{
    public static class FeatureExtensions
    {
        public static Feature UseBdd<TService>(this Feature feature)
        {
            var service = feature.Container.Resolve<TService>(nameof(Bdd));
            return feature.Use(service);
        }

        public static Feature UseTrace(this Feature feature, Action<string> trace)
        {
            return feature.Use(new Tracer(trace));
        }

        public static Feature Use<TService>(this Feature feature, TService service)
        {
            feature.Container.Register<TService>(service);
            return feature;
        }
    }
}