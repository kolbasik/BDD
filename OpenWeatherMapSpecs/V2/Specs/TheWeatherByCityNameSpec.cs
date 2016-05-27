using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenWeatherMap;

namespace OpenWeatherMapSpecs.V2.Specs
{
    public sealed class TheWeatherByCityNameSpec : OpenWeatherMapServiceSpec
    {
        public string CityName { get; private set; }
        public string CountryName { get; private set; }
        public OpenWeatherMapResult Result { get; private set; }

        public void Сity_is_cityName(string cityName)
        {
            if (cityName == null) throw new ArgumentNullException("cityName");
            CityName = cityName;
        }

        public void Сountry_is_countryName(string countryName)
        {
            if (countryName == null) throw new ArgumentNullException("countryName");
            CountryName = countryName;
        }

        public async Task When_I_have_asked_the_forecast()
        {
            Result = await OpenWeatherMapService.GetWeatherForecastAsync(CityName, CountryName).ConfigureAwait(false);
        }

        public void Then_I_will_get_the_wheather_forecast()
        {
            Assert.IsNotNull(Result);
        }

        public void The_forecast_should_contain_the_city_name()
        {
            Assert.AreEqual(CityName, Result.Name);
        }

        public void The_forecast_should_contain_the_coordinates()
        {
            Assert.IsNotNull(Result.Coord);
        }

        public void The_forecast_should_contain_the_main_information()
        {
            Assert.IsNotNull(Result.Main);
        }

        public void The_forecast_should_contain_the_wheather_information()
        {
            Assert.IsNotNull(Result.Weather);
        }
    }
}