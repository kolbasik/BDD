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
        public OpenWeatherMapService() : this(
            new Uri(@"http://api.openweathermap.org/data/2.5/weather", UriKind.Absolute),
            @"077cc5480d6fd71002f3999f7b04218c")
        {
        }

        public OpenWeatherMapService(Uri serviceUri, string appId)
        {
            if (serviceUri == null) throw new ArgumentNullException(nameof(serviceUri));
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            HttpClient = new HttpClient();
            ServiceUri = serviceUri;
            AppId = appId;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        public HttpClient HttpClient { get; private set; }
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
            var json = await HttpClient.GetStringAsync(resource).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<OpenWeatherMapResult>(json);
        }

        private Uri GetResource(Dictionary<string, string> parameters)
        {
            parameters = new Dictionary<string, string>(parameters) { { @"appid", AppId } };
            var resource = new UriBuilder(ServiceUri)
            {
                Query = string.Join(@"&", parameters.Select(x => $"{x.Key}={x.Value}"))
            };
            return resource.Uri;
        }
    }
}