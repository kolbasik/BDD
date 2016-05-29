using System.ComponentModel.Composition;
using System.Threading.Tasks;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public sealed class TheWeatherByCityNameSpec
    {
        [Import]
        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public string CityName { get; set; }
        public string CountryCode { get; set; }

        [Export]
        public OpenWeatherMapResult Result { get; private set; }

        public async Task When_I_have_asked_the_forecast()
        {
            Result = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryCode).ConfigureAwait(false);
            Assert.NotNull(Result);
        }

        public void It_should_contain_the_city_name()
        {
            Assert.Equal(CityName, Result.Name);
        }
    }
}