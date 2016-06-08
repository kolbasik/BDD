using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public sealed class TheWeatherByCityIDActions
    {
        [Import]
        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public string CityID { get; set; }

        public OpenWeatherMapForecast Forecast { get; private set; }

        public void Set(string cityId)
        {
            if (cityId == null)
            {
                throw new ArgumentNullException(nameof(cityId));
            }

            CityID = cityId;
        }

        public async Task I_have_asked_the_forecast()
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