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