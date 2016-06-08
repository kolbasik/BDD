using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V1;

namespace OpenWeatherMapSpecs.Common
{
    [TestClass]
    public abstract class MsSpec<TSpec, TResult> where TSpec : class
    {
        [TestMethod]
        public void Test()
        {
            Bdd.RunSpec(this as TSpec);
        }

        public TResult Result { get; protected set; }
    }
}