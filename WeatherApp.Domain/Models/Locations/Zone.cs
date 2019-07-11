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

namespace WeatherApp.Domain.Models.Locations
{
    public class Zone
    {
        public string Name { get; set; }
        public List<Enums.Weather> WeatherConditions => WeatherBreakpoints.Select(x => x.Weather).Distinct().ToList();
        public List<WeatherBreakpoint> WeatherBreakpoints { get; set; }

        /// <summary>
        /// Given a value between 0 and 100 returns the appropriate weather condition.
        /// </summary>
        /// <param name="chancePercent">The chance percent.</param>
        /// <returns></returns>
        public Enums.Weather GetWeatherForCalculatedChance(ulong chancePercent)
        {
            return WeatherBreakpoints.OrderBy(x => x.Value).FirstOrDefault(x => chancePercent < x.Value)?.Weather
                   ?? WeatherBreakpoints.First(x => x.Value == 100).Weather;
        }
    }
}