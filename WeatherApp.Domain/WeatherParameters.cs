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
    /// Class to contain the parameters needed to search for weather conditions
    /// </summary>
    public class WeatherParameters
    {
        public string Zone { get; set; }
        public List<string> DesiredWeather { get; set; }
        public List<string> DesiredPreviousWeather { get; set; }
        public List<string> DesiredTimes { get; set; }
        public int MaxTries { get; set; }
        public int MaxMatches { get; set; }

        public WeatherParameters()
        {
            DesiredPreviousWeather = new List<string>();
            DesiredWeather = new List<string>();
            DesiredTimes = new List<string>();
        }
    }
}