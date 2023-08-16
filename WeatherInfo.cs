namespace pvcWeatherBot
{
    // Temperature
    public class WeatherInfo
    {
        public float Temp { get; set; }
        public float Feels_like { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; } // Percents
    }
    //Wind
    public class WindInfo 
    {
        public float Speed { get; set; }
    }
    //Precipitation
    public class PrecipitationInfo 
    {
        public string Description { get; set; }
    }
    
}
