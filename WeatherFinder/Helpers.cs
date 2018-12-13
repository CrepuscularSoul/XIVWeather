using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WeatherFinder
{
    public static class Helpers
    {
        /// <summary>
        /// Get the resource id for the appropriate weather icon
        /// </summary>
        public static int GetWeatherIconIdFromName(string str)
        {
            switch (str)
            {
                case "Blizzards":
                    return Resource.Drawable.Blizzards;
                case "Clear Skies":
                    return Resource.Drawable.ClearSkies;
                case "Clouds":
                    return Resource.Drawable.Clouds;
                case "Dust Storms":
                    return Resource.Drawable.DustStorms;
                case "Fair Skies":
                    return Resource.Drawable.FairSkies;
                case "Fog":
                    return Resource.Drawable.Fog;
                case "Gales":
                    return Resource.Drawable.Gales;
                case "Gloom":
                    return Resource.Drawable.Gloom;
                case "Heat Waves":
                    return Resource.Drawable.HeatWaves;
                case "Rain":
                    return Resource.Drawable.Rain;
                case "Showers":
                    return Resource.Drawable.Showers;
                case "Snow":
                    return Resource.Drawable.Snow;
                case "Thunder":
                    return Resource.Drawable.Thunder;
                case "Thunderstorms":
                    return Resource.Drawable.Thunderstorms;
                case "Umbral Static":
                    return Resource.Drawable.UmbralStatic;
                case "Umbral Wind":
                    return Resource.Drawable.UmbralWind;
                case "Wind":
                    return Resource.Drawable.Wind;
                default:
                    return default;
            }
        }

        /// <summary>
        /// Format the eorzean hour to appear as XX:XX
        /// </summary>
        public static string FormatEorzeaHour(string hour)
        {
            if (hour.Length == 1)
                hour = "0" + hour;
            return $"{hour}:00";
        }
    }

    /// <summary>
    /// Extension methods used in the application
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Given a date get the time portion as a string, with 2 digits for hours and minutes
        /// </summary>
        public static string GetTimePortion(this DateTime d)
        {
            return $"{(d.Hour < 10 ? "0" : "")}{d.Hour}:{(d.Minute < 10 ? "0" : "")}{d.Minute}";
        }
    }
}