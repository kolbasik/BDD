/*using System;
using System.Threading.Tasks;
using NBDD.V2;
using OpenWeatherMap;
using OpenWeatherMapSpecs.V2.Specs;
using Xunit;
using Xunit.Abstractions;

namespace OpenWeatherMapSpecs.V2
{
    public class TheWeatherByCityIDTest : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public TheWeatherByCityIDTest(ITestOutputHelper output)
        {
            _output = output;
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.FromConfig());
        }

        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }

        //[Theory]
        //[InlineData(@"London", @"UK")]
        //[InlineData(@"Kyiv", @"UA")]
        //public Task Test(string cityName, string countryCode)
        //{
        //    return Bdd.Feature(
        //            AsA: @"developer",
        //            IWant: @"to get the weather forecast by city id",
        //            SoThat: @"he will be able to plan holidays.")
        //        .UseTrace(_output.WriteLine)
        //        .Use(OpenWeatherMapService)
        //        .Describe(a =>
        //        {
        //            a.Scenario()
        //            .Use<TheWeatherByCityNameSpec>()
        //                .Given(@"the city is $CityName", x => x.CityName = cityName)
        //                    .And("the country is $CountryCode", x => x.CountryCode = countryCode)
        //                .When(@"I have asked the forecast", x => x.When_I_have_asked_the_forecast())
        //            .Use<TheWeatherForecastResultSpec>()
        //                .Then(@"it should contain the coordinates", x => x.It_should_contain_the_coordinates())
        //            .Use<TheWeatherByCityIDSpec>()
        //                .Given($"the city is {cityName}", x => x.CityID = cityName)
        //                    .And($"the country is {countryCode}", x => x.CountryCode = countryCode)
        //                .When(@"I have asked the forecast", x => x.When_I_have_asked_the_forecast())
        //                .Then(@"it should contain the city name", x => x.It_should_contain_the_city_name())
        //                .Use<TheWeatherForecastResultSpec>()
        //                .And(@"it should contain the coordinates", x => x.It_should_contain_the_coordinates())
        //                .And(@"it should contain the main information", x => x.It_should_contain_the_main_information())
        //                .And(@"it should contain the wheather information", x => x.It_should_contain_the_wheather_information());
        //        })
        //        .TestAsync();
        //}
    }
}*/