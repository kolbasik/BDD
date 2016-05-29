using System.ComponentModel.Composition;
using OpenWeatherMap;
using Xunit;

namespace OpenWeatherMapSpecs.V2.Specs
{
    [Export]
    public sealed class TheWeatherForecastSpec
    {
        [Import]
        public OpenWeatherMapResult Forecast { get; set; }

        public void It_should_contain_the_coordinates()
        {
            Assert.NotNull(Forecast.Coord);
        }

        public void It_should_contain_the_main_information()
        {
            Assert.NotNull(Forecast.Main);
        }

        public void It_should_contain_the_wheather_information()
        {
            Assert.NotNull(Forecast.Weather);
        }
    }
}