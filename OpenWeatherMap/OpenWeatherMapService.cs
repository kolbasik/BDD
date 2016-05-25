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
        public OpenWeatherMapService()
        {
            HttpClient = new HttpClient();
            ServiceUri = new Uri("http://api.openweathermap.org/data/2.5/weather", UriKind.Absolute);
            AppId = "077cc5480d6fd71002f3999f7b04218c";
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public HttpClient HttpClient { get; set; }
        public Uri ServiceUri { get; private set; }
        public string AppId { get; private set; }

        public Task<OpenWeatherMapResult> GetWeatherForecastAsync(string city, string country)
        {
            var parameters = new Dictionary<string, string> { { "q", city + "," + country } };
            return ExecuteAsync(parameters);
        }

        public Task<OpenWeatherMapResult> GetWeatherForecastAsync(string id)
        {
            var parameters = new Dictionary<string, string> { { "id", id } };
            return ExecuteAsync(parameters);
        }

        public Task<OpenWeatherMapResult> GetWeatherForecastAsync(decimal lat, decimal lon)
        {
            var parameters = new Dictionary<string, string>
            {
                { "lat", lat.ToString(CultureInfo.InvariantCulture) },
                { "lon", lon.ToString(CultureInfo.InvariantCulture) }
            };
            return ExecuteAsync(parameters);
        }

        private async Task<OpenWeatherMapResult> ExecuteAsync(Dictionary<string, string> parameters)
        {
            var resource = GetResource(parameters);

            var json = await HttpClient.GetStringAsync(resource.Uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<OpenWeatherMapResult>(json);
        }

        private UriBuilder GetResource(Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>(parameters) { { "appid", AppId } };
            var resource = new UriBuilder(ServiceUri);
            resource.Query = string.Join("&", parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
            return resource;
        }
    }
}