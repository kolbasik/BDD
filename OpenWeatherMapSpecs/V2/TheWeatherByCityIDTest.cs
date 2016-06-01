using System.Threading.Tasks;
using NBDD.V2;
using OpenWeatherMap;
using OpenWeatherMapSpecs.V2.Specs;
using Xunit;
using Xunit.Abstractions;

namespace OpenWeatherMapSpecs.V2
{
    public class TheWeatherByCityIDTest : OpenWeatherMapServiceTest
    {
        private readonly ITestOutputHelper _output;

        public TheWeatherByCityIDTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(@"London", @"UK")]
        [InlineData(@"Paris", @"FR")]
        public Task Test(string cityName, string countryCode)
        {
            return Bdd.Feature(
                    AsA: @"developer",
                    IWant: @"to get the weather forecast by city id",
                    SoThat: @"he will be able to plan holidays.")
                .Use(OpenWeatherMapService)
                .Describe(a =>
                {
                    a.Scenario()
                        .Bind(s => s.Prop(@"CityName", cityName))
                        .Bind(s => s.Prop(@"CountryCode", countryCode))

                        .Use<TheWeatherByCityNameActions>()
                        .Given(@"the city is $CityName", (x, s) => x.CityName = s.Prop<string>(@"CityName"))
                        .And(@"the country is $CountryCode", (x, s) => x.CountryCode = s.Prop<string>(@"CountryCode"))
                        .When((x, s) => x.When_I_have_asked_the_forecast())
                        .Then((x, s) => x.It_should_contain_the_city_id())
                        .Bind((x, s) => s.Prop(@"CityID", x.Forecast.Id))

                        .Use<TheWeatherByCityIDActions>()
                        .Given(@"the city is $CityID", (x, s) => x.CityID = s.Prop<string>(@"CityID"))
                        .When((x, s) => x.When_I_have_asked_the_forecast())
                        .Then((x, s) => x.It_should_contain_the_city_id())
                        .Bind((x, s) => s.Prop(@"Forecast", x.Forecast))

                        .Use<TheWeatherForecastResultActions>()
                        .Given(@"The forecast", (x, s) => x.Forecast = s.Prop<OpenWeatherMapForecast>(@"Forecast"))
                        .Then(@"it should contain the $CityName city name", (x, s) => x.It_should_contain_the_city_name(s.Prop<string>(@"CityName")))
                        .And((x, s) => x.It_should_contain_the_coordinates())
                        .And((x, s) => x.It_should_contain_the_main_information())
                        .And((x, s) => x.It_should_contain_the_wheather_information());
                })
                .UseTrace(_output.WriteLine)
                .TestAsync();
        }
    }
}