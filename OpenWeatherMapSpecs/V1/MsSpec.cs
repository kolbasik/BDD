using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V1;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V1
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

    [TestClass]
    public abstract class OpenWeatherMapServiceSpec<TSpec> : MsSpec<TSpec, OpenWeatherMapForecast>, IDisposable
        where TSpec : class
    {
        protected OpenWeatherMapService OpenWeatherMapService { get; private set; }

        protected OpenWeatherMapServiceSpec()
        {
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.FromConfig());
        }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }
    }
}