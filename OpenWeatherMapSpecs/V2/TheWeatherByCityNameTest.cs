using System;
using System.Threading.Tasks;
using NBDD.V2;
using OpenWeatherMap;
using OpenWeatherMapSpecs.V2.Specs;
using Xunit;
using Xunit.Abstractions;

namespace OpenWeatherMapSpecs.V2
{
    public class TheWeatherByCityNameTest : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public TheWeatherByCityNameTest(ITestOutputHelper output)
        {
            _output = output;
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.FromConfig());
        }

        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }

        [Theory]
        [InlineData(@"London", @"UK")]
        [InlineData(@"Kyiv", @"UA")]
        public Task Test(string cityName, string countryCode)
        {
            return Bdd.Feature(
                    AsA: @"developer",
                    IWant: @"to get the weather forecast by city name",
                    SoThat: @"he will be able to plan holidays.")
                .UseTrace(_output.WriteLine)
                .Use(OpenWeatherMapService)
                .Describe(a =>
                {
                    a.Scenario()
                        .Bind(s => s.Props[@"CityName"] = cityName)
                        .Bind(s => s.Props[@"CountryCode"] = countryCode)

                        .Use<TheWeatherByCityNameSpec>()
                        .Given(@"the city is $CityName", (x, s) => x.CityName = (string) s.Props[@"CityName"])
                        .And(@"the country is $CountryCode", (x, s) => x.CountryCode = (string) s.Props[@"CountryCode"])
                        .When(@"I have asked the forecast", (x, s) => x.When_I_have_asked_the_forecast())
                        .Then(@"it should contain the city name", (x, s) => x.It_should_contain_the_city_name())
                        .Bind((x, s) => s.Container.Register(x.Forecast))

                        .Use<TheWeatherForecastResultSpec>()
                        .And(@"it should contain the coordinates", (x, s) => x.It_should_contain_the_coordinates())
                        .And(@"it should contain the main information", (x, s) => x.It_should_contain_the_main_information())
                        .And(@"it should contain the wheather information", (x, s) => x.It_should_contain_the_wheather_information());
                })
                .TestAsync();
        }
    }
}