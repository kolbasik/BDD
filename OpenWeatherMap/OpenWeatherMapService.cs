using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenWeatherMap
{
    public sealed class OpenWeatherMapService : IDisposable
    {
        public OpenWeatherMapService(OpenWeatherMapConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            HttpClient = new HttpClient();
            Config = config;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public HttpClient HttpClient { get; }
        public OpenWeatherMapConfig Config { get; }


        public Task<OpenWeatherMapForecast> GetWeatherForecastAsync(string city, string country)
        {
            var parameters = new Dictionary<string, string> { { "q", city + "," + country } };
            return ExecuteAsync(parameters);
        }

        public Task<OpenWeatherMapForecast> GetWeatherForecastAsync(string id)
        {
            var parameters = new Dictionary<string, string> { { "id", id } };
            return ExecuteAsync(parameters);
        }

        public Task<OpenWeatherMapForecast> GetWeatherForecastAsync(decimal lat, decimal lon)
        {
            var parameters = new Dictionary<string, string>
            {
                { "lat", lat.ToString(CultureInfo.InvariantCulture) },
                { "lon", lon.ToString(CultureInfo.InvariantCulture) }
            };
            return ExecuteAsync(parameters);
        }

        private async Task<OpenWeatherMapForecast> ExecuteAsync(Dictionary<string, string> parameters)
        {
            var resource = GetResource(parameters);
            var json = await HttpClient.GetStringAsync(resource).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<OpenWeatherMapForecast>(json);
        }

        private Uri GetResource(Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>(parameters) { { @"appid", Config.AppId } };
            var resource = new UriBuilder(Config.ServiceUri)
            {
                Query = string.Join(@"&", parameters.Select(x => $"{x.Key}={x.Value}"))
            };
            return resource.Uri;
        }
    }
}