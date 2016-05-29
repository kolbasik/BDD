using OpenWeatherMap.Data;

namespace OpenWeatherMap
{
    public sealed class OpenWeatherMapForecast
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Main Main { get; set; }
        public Coord Coord { get; set; }
        public Weather[] Weather { get; set; }
    }
}