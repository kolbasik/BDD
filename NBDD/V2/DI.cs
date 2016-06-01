using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public sealed class DI : IDisposable
    {
        public static readonly DI Global;

        static DI()
        {
            Global = new DI(new CompositionContainer(new ApplicationCatalog()));
        }

        [DebuggerHidden]
        public DI(CompositionContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            Composition = new CompositionContainer(new ApplicationCatalog(), container);
            Composition.ComposeExportedValue(Composition);
        }

        public CompositionContainer Composition { get; }

        [DebuggerHidden]
        public void Dispose()
        {
            Composition.Dispose();
        }

        [DebuggerHidden]
        public DI Scope()
        {
            return new DI(Composition);
        }

        [DebuggerHidden]
        public DI Register<TService>(string contractName, TService service)
        {
            Composition.ComposeExportedValue(contractName, service);
            return this;
        }

        [DebuggerHidden]
        public DI Register<TService>(TService service)
        {
            Composition.ComposeExportedValue(service);
            return this;
        }

        [DebuggerHidden]
        public TService Resolve<TService>()
        {
            return Composition.GetExportedValue<TService>();
        }

        [DebuggerHidden]
        public TService Resolve<TService>(string contractName)
        {
            return Composition.GetExportedValue<TService>(contractName);
        }

        [DebuggerHidden]
        public TService ResolveOrDefault<TService>()
        {
            return Composition.GetExportedValueOrDefault<TService>();
        }

        [DebuggerHidden]
        public TService ResolveOrDefault<TService>(string contractName = null)
        {
            return Composition.GetExportedValueOrDefault<TService>(contractName);
        }
    }
}