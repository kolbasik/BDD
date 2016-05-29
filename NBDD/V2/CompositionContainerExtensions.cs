using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class CompositionContainerExtensions
    {
        [DebuggerHidden]
        public static CompositionContainer Register<TService>(this CompositionContainer container, string contractName, TService service)
        {
            container.ComposeExportedValue<TService>(contractName, service);
            return container;
        }

        [DebuggerHidden]
        public static CompositionContainer Register<TService>(this CompositionContainer container, TService service)
        {
            container.ComposeExportedValue<TService>(service);
            return container;
        }

        [DebuggerHidden]
        public static TService Resolve<TService>(this CompositionContainer container, string contractName)
        {
            return container.GetExportedValue<TService>(contractName);
        }

        [DebuggerHidden]
        public static TService Resolve<TService>(this CompositionContainer container)
        {
            var service = container.GetExportedValueOrDefault<TService>();
            if (object.Equals(service, default(TService)))
            {
                return container.Resolve<TService>(nameof(Bdd));
            }
            return service;
        }

        [DebuggerHidden]
        public static CompositionContainer Scope(this CompositionContainer parent)
        {
            var container = new CompositionContainer(new ApplicationCatalog(), parent);
            return container.Register(container);
        }
    }
}