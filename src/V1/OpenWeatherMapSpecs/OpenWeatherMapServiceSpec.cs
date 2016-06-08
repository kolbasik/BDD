using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenWeatherMap;
using OpenWeatherMapSpecs.Common;

namespace OpenWeatherMapSpecs
{
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