using System.Diagnostics;

namespace NBDD.V2
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public static class Bdd
    {
        [DebuggerHidden]
        public static Feature Feature(string skip = null, string AsA = null, string IWant = null, string SoThat = null)
        {
            var feature = new Feature(DI.Global);
            feature.AsA = AsA;
            feature.IWant = IWant;
            feature.SoThat = SoThat;
            return feature;
        }

        [DebuggerHidden]
        public static void Trace(string message) => System.Diagnostics.Trace.WriteLine(message);
    }
}