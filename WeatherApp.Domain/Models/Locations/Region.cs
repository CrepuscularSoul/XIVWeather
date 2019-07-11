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
    public class Region
    {
        public string Name { get; set; }
        public List<Zone> Zones { get; set; }
    }

    public static class RegionExtensions
    {
        public static Zone GetZone(this List<Region> regions, string zoneName)
            => regions.First(x => x.Zones.Any(y => y.Name == zoneName))
                .Zones.First(x => x.Name == zoneName);
    }
}