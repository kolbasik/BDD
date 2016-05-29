using System.ComponentModel.Composition;
using System.Threading.Tasks;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public class TheWeatherByGeographicCoordinatesActions
    {
        [Import]
        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public decimal Lat { get; set; }
        public decimal Lon { get; set; }

        public OpenWeatherMapForecast Forecast { get; private set; }

        public async Task When_I_have_asked_the_forecast()
        {
            Forecast = await OpenWeatherMapService.GetWeatherForecastAsync(Lat, Lon).ConfigureAwait(false);
            Assert.NotNull(Forecast);
        }

        public void It_should_contain_the_geographic_coordinates()
        {
            Assert.Equal(Lat, Forecast.Coord.Lat);
            Assert.Equal(Lon, Forecast.Coord.Lon);
        }
    }
}