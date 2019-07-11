
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace WeatherApp.Domain.Models
{
    public static class Enums
    {
        static Enums()
        {
            WeatherMapping = new Dictionary<Weather, string>();

            var enumValues = Enum.GetValues(typeof(Weather));
            foreach (var value in enumValues)
            {
                var constant = typeof(Constants.Weather).GetFields()
                    .FirstOrDefault(x => x.Name == Enum.GetName(typeof(Weather), value));
                var constantValue = constant?.GetValue(constant);
                WeatherMapping.Add((Weather)value, constantValue?.ToString());
            }
        }

        public enum Weather
        {
            Blizzards,
            ClearSkies,
            Clouds,
            DustStorms,
            FairSkies,
            Fog,
            Gales,
            Gloom,
            HeatWaves,
            Rain,
            Showers,
            Snow,
            Thunder,
            Thunderstorms,
            UmbralStatic,
            UmbralWind,
            Wind,
        }

        public static Dictionary<Weather, string> WeatherMapping { get; }
    }
}