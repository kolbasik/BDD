using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace NBDD.V2
{
    public static class CompositionContainerExtensions
    {
        public static CompositionContainer Register<TService>(this CompositionContainer container, string contractName, TService service)
        {
            container.ComposeExportedValue<TService>(contractName, service);
            return container;
        }

        public static CompositionContainer Register<TService>(this CompositionContainer container, TService service)
        {
            container.ComposeExportedValue<TService>(service);
            return container;
        }

        public static TService Resolve<TService>(this CompositionContainer container, string contractName)
        {
            return container.GetExportedValue<TService>(contractName);
        }

        public static TService Resolve<TService>(this CompositionContainer container)
        {
            var service = container.GetExportedValueOrDefault<TService>();
            if (object.Equals(service, default(TService)))
            {
                return container.Resolve<TService>(nameof(Bdd));
            }
            return service;
        }

        public static CompositionContainer Scope(this CompositionContainer parent)
        {
            var container = new CompositionContainer(new ApplicationCatalog(), parent);
            return container.Register(container);
        }
    }
}