using System.Collections.Generic;

namespace pvcWeatherBot
{
        public class WeatherResponse
        {
            public WeatherInfo Main { get; set; }
            public WindInfo Wind { get; set;}
        public List<Weather> weather { get; set; }

        public string Name { get; set; }
        }
    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
}
