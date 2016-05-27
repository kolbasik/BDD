using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBDD.V2;
using OpenWeatherMapSpecs.V2.Specs;

namespace OpenWeatherMapSpecs.V2
{
    [TestClass]
    public class GetTheWeatherByCityNameSpec
    {
        [TestMethod]
        public void Demo()
        {
            Bdd.Feature(
                    AsA: @"developer",
                    IWant: @"to get the weather forecast by city name",
                    SoThat: @"he will be able to plan holidays.")
                .Describe(the =>
                {
                    the.Scenario()
                        .Use<TheWeatherByCityNameSpec>()
                            .Given("The city is London", x => x.Сity_is__cityName(@"London"))
                                .And("  And the country is UK", x => x.Сountry_is__countryName(@"uk"))
                            .When("When I have asked the forecast", x => x.When_I_have_asked_the_forecast())
                            .Then("Then I will get the wheather forecast", x => x.Then_I_will_get_the_wheather_forecast())
                                .And("  And the forecast should contain the city name", x => x.The_forecast_should_contain_the_city_name())
                                .And("  And the forecast should contain the coordinates", x => x.The_forecast_should_contain_the_coordinates())
                                .And("  And the forecast should contain the main information", x => x.The_forecast_should_contain_the_main_information())
                                .And("  And the forecast should contain the wheather information", x => x.The_forecast_should_contain_the_wheather_information());
                })
                .Play();
        }
    }
}