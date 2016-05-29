using System;
using System.Threading.Tasks;
using NBDD.V2;
using OpenWeatherMap;
using OpenWeatherMapSpecs.V2.Specs;
using Xunit;
using Xunit.Abstractions;

namespace OpenWeatherMapSpecs.V2
{
    public class GetTheWeatherByCityNameSpec : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public GetTheWeatherByCityNameSpec(ITestOutputHelper output)
        {
            _output = output;
            OpenWeatherMapService = new OpenWeatherMapService(OpenWeatherMapConfig.FromConfig());
        }

        public void Dispose()
        {
            OpenWeatherMapService.Dispose();
        }

        public OpenWeatherMapService OpenWeatherMapService { get; set; }

        [Theory]
        [InlineData(@"London", @"UK")]
        [InlineData(@"Kyiv", @"UA")]
        public Task Demo(string cityName, string countryCode)
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
                        .Use<TheWeatherByCityNameSpec>()
                        .Given($"the city is {cityName}", x => x.CityName = cityName)
                            .And($"the country is {countryCode}", x => x.CountryCode = countryCode)
                        .When(@"I have asked the forecast", x => x.When_I_have_asked_the_forecast())
                        .Then(@"it should contain the city name", x => x.It_should_contain_the_city_name())
                            .Use<TheWeatherForecastSpec>()
                            .And(@"it should contain the coordinates", x => x.It_should_contain_the_coordinates())
                            .And(@"it should contain the main information", x => x.It_should_contain_the_main_information())
                            .And(@"it should contain the wheather information", x => x.It_should_contain_the_wheather_information());
                })
                .TestAsync();
        }
    }
}