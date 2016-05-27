using System;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V2.Specs
{
    public abstract class OpenWeatherMapServiceSpec : IDisposable
    {
        protected OpenWeatherMapService OpenWeatherMapService { get; private set; }

        protected OpenWeatherMapServiceSpec()
        {
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.Test());
        }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }
    }
}