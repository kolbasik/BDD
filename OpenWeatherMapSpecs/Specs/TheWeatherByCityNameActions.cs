using System.ComponentModel.Composition;
using System.Threading.Tasks;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public sealed class TheWeatherByCityNameActions
    {
        [Import]
        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public string CityName { get; set; }
        public string CountryCode { get; set; }

        public OpenWeatherMapForecast Forecast { get; private set; }

        public void Set(string cityName, string countryCode)
        {
            CityName = cityName;
            CountryCode = countryCode;
        }

        public async Task I_have_asked_the_forecast()
        {
            Forecast = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryCode).ConfigureAwait(false);
            Assert.NotNull(Forecast);
        }

        public void It_should_contain_the_city_id()
        {
            Assert.NotNull(Forecast.Id);
        }

        public void It_should_contain_the_city_name()
        {
            Assert.Equal(CityName, Forecast.Name);
        }

        public void It_should_contain_the_geographic_coordinates()
        {
            Assert.NotNull(Forecast.Coord);
        }

        public void It_should_contain_the_country_code()
        {
        }
    }
}