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
        public static TService Resolve<TService>(this CompositionContainer container)
        {
            return container.GetExportedValue<TService>();
        }

        [DebuggerHidden]
        public static TService Resolve<TService>(this CompositionContainer container, string contractName)
        {
            return container.GetExportedValue<TService>(contractName);
        }

        [DebuggerHidden]
        public static TService ResolveOrDefault<TService>(this CompositionContainer container)
        {
            return container.GetExportedValueOrDefault<TService>();
        }

        [DebuggerHidden]
        public static TService ResolveOrDefault<TService>(this CompositionContainer container, string contractName = null)
        {
            return container.GetExportedValueOrDefault<TService>(contractName);
        }

        [DebuggerHidden]
        public static CompositionContainer Scope(this CompositionContainer parent)
        {
            var container = new CompositionContainer(new ApplicationCatalog(), parent);
            return container.Register(container);
        }
    }
}