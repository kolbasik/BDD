using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V2.Specs
{
    public sealed class TheWeatherByCityNameSpec : OpenWeatherMapServiceSpec
    {
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public OpenWeatherMapResult Result { get; private set; }

        public async Task When_I_have_asked_the_forecast()
        {
            Result = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryCode).ConfigureAwait(false);
            Assert.IsNotNull(Result);
        }

        public void It_should_contain_the_city_name()
        {
            Assert.AreEqual(CityName, Result.Name);
        }

        public void It_should_contain_the_coordinates()
        {
            Assert.IsNotNull(Result.Coord);
        }

        public void It_should_contain_the_main_information()
        {
            Assert.IsNotNull(Result.Main);
        }

        public void It_should_contain_the_wheather_information()
        {
            Assert.IsNotNull(Result.Weather);
        }
    }
}