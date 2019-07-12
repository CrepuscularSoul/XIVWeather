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
    /// A Region in Final Fantasy XIV such as The Black Shroud.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The Name of the region.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The zones that belong to the region.
        /// </summary>
        public List<Zone> Zones { get; set; }
    }

    /// <summary>
    /// Extension method to clean up the task of getting a specific zone out of
    /// a region collection of all regions.
    /// </summary>
    public static class RegionExtensions
    {
        public static Zone GetZone(this List<Region> regions, string zoneName)
            => regions.First(x => x.Zones.Any(y => y.Name == zoneName))
                .Zones.First(x => x.Name == zoneName);
    }
}