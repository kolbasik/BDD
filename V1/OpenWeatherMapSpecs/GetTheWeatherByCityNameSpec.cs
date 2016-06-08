using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V1;

namespace OpenWeatherMapSpecs
{
    [TestClass]
    public class GetTheWeatherByCityNameSpec : OpenWeatherMapServiceSpec<GetTheWeatherByCityNameSpec>
    {
        public string CityName { get; set; }
        public string CountryName { get; set; }

        [Bdd.Given]
        public void Given_I_want_to_know_the_weather_for_London()
        {
            CityName = "London";
            CountryName = "uk";
        }

        [Bdd.When]
        public async Task When_I_have_asked_the_forecast_by_city_name()
        {
            Result = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryName).ConfigureAwait(false);
        }

        [Bdd.Then]
        public void Then_I_will_get_the_wheather_forecast()
        {
            Assert.IsNotNull(Result);
        }

        [Bdd.Then]
        public void And_the_forecast_will_have_the_city_name()
        {
            Assert.AreEqual<string>(CityName, Result.Name);
        }

        [Bdd.Then]
        public void And_the_forecast_will_have_the_coordinates()
        {
            Assert.IsNotNull(Result.Coord);
        }

        [Bdd.Then]
        public void And_the_forecast_will_have_the_main_information()
        {
            Assert.IsNotNull(Result.Main);
        }

        [Bdd.Then]
        public void And_the_forecast_will_have_the_wheather_information()
        {
            Assert.IsNotNull(Result.Weather);
        }
    }
}