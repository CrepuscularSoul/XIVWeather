using System;

namespace WeatherApp.Domain.Models.Weather
{
    /// <summary>
    /// Class containing details about an upcoming weather window
    /// </summary>
    public class WeatherResult
    {
        public string Zone { get; set; }
        public string PreviousWeather { get; set; }
        public string CurrentWeather { get; set; }
        public string StartTime { get; set; }
        public DateTime TimeOfWeather { get; set; }
    }
}