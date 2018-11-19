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

namespace WeatherApp.Domain
{
    /// <summary>
    /// Class wrapping a zone name and function for getting the weather based on a value between 0 and 100
    /// </summary>
    public class WeatherChance
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WeatherChance(string zone, Func<ulong, string> getWeatherFunc)
        {
            ZoneName = zone;
            _getWeatherForChanceFunc = getWeatherFunc;
        }

        /// <summary>
        /// The name of the zone.
        /// </summary>
        public string ZoneName { get; set; }

        /// <summary>
        /// Function for getting the name of the weather window for a given chance (between 0 and 100)
        /// </summary>
        private readonly Func<ulong, string> _getWeatherForChanceFunc;

        /// <summary>
        /// Public method wrapping the weather function supplied at time of object creation.
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        public string GetWeather(ulong chance)
        {
            return _getWeatherForChanceFunc.Invoke(chance);
        }
    }
}