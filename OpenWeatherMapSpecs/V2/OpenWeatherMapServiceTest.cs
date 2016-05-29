using System;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V2
{
    public abstract class OpenWeatherMapServiceTest : IDisposable
    {
        protected OpenWeatherMapService OpenWeatherMapService { get; private set; }

        protected OpenWeatherMapServiceTest()
        {
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.FromConfig());
        }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }
    }
}