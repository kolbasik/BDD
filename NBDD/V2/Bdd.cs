using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;

namespace NBDD.V2
{
    public static class Bdd
    {
        internal static readonly Task Done = Task.FromResult(true);
        public static readonly CompositionContainer Container;

        static Bdd()
        {
            Container = new CompositionContainer().Scope();
            Container.Register(nameof(Bdd), new Tracer(Trace));
        }

        public static Feature Feature(string skip = null, string AsA = null, string IWant = null, string SoThat = null)
        {
            return new Feature { AsA = AsA, IWant = IWant, SoThat = SoThat };
        }

        public static void Trace(string message) => System.Diagnostics.Trace.WriteLine(message);
    }
}