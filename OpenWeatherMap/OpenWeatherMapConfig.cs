using System;
using System.Configuration;

namespace OpenWeatherMap
{
    public sealed class OpenWeatherMapConfig
    {
        public OpenWeatherMapConfig(Uri serviceUri, string appId)
        {
            if (serviceUri == null) throw new ArgumentNullException(nameof(serviceUri));
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            ServiceUri = serviceUri;
            AppId = appId;
        }

        public Uri ServiceUri { get; private set; }
        public string AppId { get; private set; }

        public static OpenWeatherMapConfig FromConfig()
        {
            return new OpenWeatherMapConfig(new Uri(ConfigurationManager.AppSettings.Get(@"OpenWeatherMap:ServiceUri"), UriKind.Absolute), ConfigurationManager.AppSettings.Get(@"OpenWeatherMap:AppId"));
        }
    }
}