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
    /// <summary>
    /// A zone with Final Fantasy XIV such as The Central Shroud.
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// The Name of the zone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The weather conditions that are possible within the zone.
        /// </summary>
        public List<Enums.Weather> WeatherConditions => WeatherBreakpoints.Select(x => x.Weather).Distinct().ToList();

        /// <summary>
        /// The breakpoints at which each available weather condition stops being applicable.
        /// </summary>
        public List<WeatherBreakpoint> WeatherBreakpoints { get; set; }

        /// <summary>
        /// Given a value between 0 and 100 returns the appropriate weather condition.
        /// </summary>
        public Enums.Weather GetWeatherForCalculatedChance(ulong chancePercent)
        {
            return WeatherBreakpoints.OrderBy(x => x.Value).FirstOrDefault(x => chancePercent < x.Value)?.Weather
                   ?? WeatherBreakpoints.First(x => x.Value == 100).Weather;
        }
    }
}