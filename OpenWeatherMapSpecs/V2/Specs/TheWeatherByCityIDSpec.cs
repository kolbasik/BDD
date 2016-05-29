using System.ComponentModel.Composition;
using System.Threading.Tasks;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public sealed class TheWeatherByCityIDSpec
    {
        [Import]
        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public string CityID { get; set; }

        public OpenWeatherMapResult Forecast { get; private set; }

        public async Task When_I_have_asked_the_forecast()
        {
            Forecast = await OpenWeatherMapService.GetWeatherForecastAsync(CityID).ConfigureAwait(false);
            Assert.NotNull(Forecast);
        }

        public void It_should_contain_the_city_id()
        {
            Assert.Equal(CityID, Forecast.Id);
        }
    }
}