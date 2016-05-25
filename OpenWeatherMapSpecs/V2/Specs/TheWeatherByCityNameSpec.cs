using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V2;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V2.Specs
{
    public sealed class TheWeatherByCityNameSpec : OpenWeatherMapServiceSpec
    {
        public static void Execute(string cityName, string countryName)
        {
            Bdd.Scen<TheWeatherByCityNameSpec>()
                .When(x => x.Сity_is__cityName(cityName))
                .And(x => x.Сountry_is__countryName(countryName))
                .When(x => x.When_I_have_asked_the_forecast())
                .Then(x => x.Then_I_will_get_the_wheather_forecast())
                .And(x => x.The_forecast_should_contain_the_city_name())
                .And(x => x.The_forecast_should_contain_the_coordinates())
                .And(x => x.The_forecast_should_contain_the_main_information())
                .And(x => x.The_forecast_should_contain_the_wheather_information())
                .Execute();
        }

        public string CityName { get; private set; }
        public string CountryName { get; private set; }
        public OpenWeatherMapResult Result { get; private set; }

        [Bdd.Given]
        public void Сity_is__cityName(string cityName)
        {
            if (cityName == null) throw new ArgumentNullException("cityName");
            CityName = cityName;
        }

        [Bdd.Given]
        public void Сountry_is__countryName(string countryName)
        {
            if (countryName == null) throw new ArgumentNullException("countryName");
            CountryName = countryName;
        }

        [Bdd.When]
        public async Task When_I_have_asked_the_forecast()
        {
            Result = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryName).ConfigureAwait(false);
        }

        [Bdd.Then]
        public void Then_I_will_get_the_wheather_forecast()
        {
            Assert.IsNotNull(Result);
        }

        [Bdd.Then]
        public void The_forecast_should_contain_the_city_name()
        {
            Assert.AreEqual(CityName, Result.Name);
        }

        [Bdd.Then]
        public void The_forecast_should_contain_the_coordinates()
        {
            Assert.IsNotNull(Result.Coord);
        }

        [Bdd.Then]
        public void The_forecast_should_contain_the_main_information()
        {
            Assert.IsNotNull(Result.Main);
        }

        [Bdd.Then]
        public void The_forecast_should_contain_the_wheather_information()
        {
            Assert.IsNotNull(Result.Weather);
        }
    }
}