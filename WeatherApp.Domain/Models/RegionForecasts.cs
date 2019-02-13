using System.Collections.Generic;

namespace WeatherApp.Domain.Models
{
    public class RegionForecast
    {
        public string Region { get; set; }
        public List<ZoneForecast> ZoneForecasts { get; set; }

        public RegionForecast(string region)
        {
            Region = region;
            ZoneForecasts = new List<ZoneForecast>();
        }

        public void AddZoneForecast(string zone, int orderValue, List<WeatherResult> results)
        {
            ZoneForecasts.Add(new ZoneForecast(zone, orderValue, results));
        }

    }

    public class ZoneForecast
    {
        public string Zone { get; set; }
        public int ZoneOrder { get; set; }
        public List<WeatherResult> WeatherResults { get; set; }

        public ZoneForecast(string zone, int orderValue, List<WeatherResult> results)
        {
            Zone = zone;
            ZoneOrder = orderValue;
            WeatherResults = results ?? new List<WeatherResult>();
        }
    }
}