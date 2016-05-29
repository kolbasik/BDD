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

        public static TService Resolve<TService>(this CompositionContainer container) where TService : class
        {
            return container.GetExportedValueOrDefault<TService>() ?? container.Resolve<TService>(nameof(Bdd));
        }

        public static CompositionContainer Scope(this CompositionContainer parent)
        {
            var container = new CompositionContainer(new ApplicationCatalog(), parent);
            return container.Register(container);
        }
    }
}