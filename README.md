[![Build status](https://ci.appveyor.com/api/projects/status/40xwv599e9q5utxb/branch/master?svg=true)](https://ci.appveyor.com/project/kolbasik/nbdd/branch/master)

# BDD
Tests with BDD style using reusable components

### Code
```csharp
public class TheWeatherByCityNameTest : OpenWeatherMapServiceTest
    {
        private readonly ITestOutputHelper _output;

        public TheWeatherByCityNameTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(@"London", @"UK")]
        [InlineData(@"Paris", @"FR")]
        [InlineData(@"Kiev", @"UA")]
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

                        .Use<TheWeatherByCityNameActions>()
                        .Given(@"the city is $CityName", (x, s) => x.CityName = (string) s.Props[@"CityName"])
                        .And(@"the country is $CountryCode", (x, s) => x.CountryCode = (string) s.Props[@"CountryCode"])
                        .When(@"I have asked the forecast", (x, s) => x.When_I_have_asked_the_forecast())
                        .Then(@"it should contain the city name", (x, s) => x.It_should_contain_the_city_name())
                        .Bind((x, s) => s.Props[@"Forecast"] = x.Forecast)

                        .Use<TheWeatherForecastResultActions>()
                        .Given(@"the forecast", (x, s) => x.Forecast = (OpenWeatherMapForecast) s.Props[@"Forecast"])
                        .Then(@"it should contain the coordinates", (x, s) => x.It_should_contain_the_coordinates())
                        .And(@"it should contain the main information", (x, s) => x.It_should_contain_the_main_information())
                        .And(@"it should contain the wheather information", (x, s) => x.It_should_contain_the_wheather_information());
                })
                .TestAsync();
        }
    }
```

### Output
```
Feature:
	As a developer
	I want to get the weather forecast by city name
	So that he will be able to plan holidays.

Scenario:

	Given the city is London PASSED 344.1ms
	 and the country is UK PASSED 2.5ms
	When I have asked the forecast PASSED 624.9ms
	Then it should contain the city name PASSED 1.7ms

	Given the forecast PASSED 0.9ms
	Then it should contain the coordinates PASSED 0.2ms
	 and it should contain the main information PASSED 0.2ms
	 and it should contain the wheather information PASSED 0.1ms
```
