using System.Diagnostics;
using System.Threading.Tasks;
using NBDD.V2;
using OpenWeatherMapSpecs.V2.Specs;
using Xunit;
using Xunit.Abstractions;

namespace OpenWeatherMapSpecs.V2
{
    public class GetTheWeatherByCityNameSpec
    {
        private readonly ITestOutputHelper _output;

        public GetTheWeatherByCityNameSpec(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(@"London", @"UK")]
        [InlineData(@"Kyiv", @"UA")]
        public Task Demo(string cityName, string countryCode)
        {
            return Bdd.Feature(
                    AsA: @"developer",
                    IWant: @"to get the weather forecast by city name",
                    SoThat: @"he will be able to plan holidays.")
                .Describe(the =>
                {
                    the.Scenario()
                        .Use<TheWeatherByCityNameSpec>()
                            .Given($"the city is {cityName}", x => x.Сity_is_cityName(cityName))
                                .And($"the country is {countryCode}", x => x.Сountry_is_countryName(countryCode))
                            .When(@"I have asked the forecast", x => x.When_I_have_asked_the_forecast())
                            .Then(@"I will get the wheather forecast", x => x.Then_I_will_get_the_wheather_forecast())
                                .And(@"the forecast should contain the city name", x => x.The_forecast_should_contain_the_city_name())
                                .And(@"the forecast should contain the coordinates", x => x.The_forecast_should_contain_the_coordinates())
                                .And(@"the forecast should contain the main information", x => x.The_forecast_should_contain_the_main_information())
                                .And(@"the forecast should contain the wheather information", x => x.The_forecast_should_contain_the_wheather_information());
                })
                .PlayAsync(_output.WriteLine);
        }
    }
}