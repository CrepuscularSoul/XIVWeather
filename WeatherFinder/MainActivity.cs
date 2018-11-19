using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain;
using ActionBar = Android.App.ActionBar;

namespace WeatherFinder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.mainLayout);
            ConfigureViews();
        }

        /// <summary>
        /// Configure the default data and events for views
        /// </summary>
        private void ConfigureViews()
        {
            var zones = WeatherService.GetZones();
            zones.Insert(0, "Select...");
            var zoneAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, zones);
            var zoneElement = FindViewById<Spinner>(Resource.Id.ZoneSpinner);
            zoneElement.Adapter = zoneAdapter;
            zoneElement.ItemSelected += ZoneSelected;

            var matchOptions = new[] {10, 20, 30, 40, 50};
            var matchAdapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleListItem1, matchOptions);
            var matchElement = FindViewById<Spinner>(Resource.Id.MaximumMatches);
            matchElement.Adapter = matchAdapter;

            var windowsOptions = new[] {1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000};
            var windowsAdapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleListItem1, windowsOptions);
            var windowsElement = FindViewById<Spinner>(Resource.Id.MaximumWindows);
            windowsElement.Adapter = windowsAdapter;

            var btn = FindViewById<Button>(Resource.Id.findWeather);
            btn.Click += FindWeather;
        }

        /// <summary>
        /// Handle the find weather click event and get the data for the user.
        /// </summary>
        private void FindWeather(object sender, EventArgs args)
        {
            var zone = FindViewById<Spinner>(Resource.Id.ZoneSpinner).SelectedItem.ToString();
            var desiredWeather = GetSelectedWeather(FindViewById<MultiSelectionSpinner>(Resource.Id.DesiredSpinner));
            var previousWeather = GetSelectedWeather(FindViewById<MultiSelectionSpinner>(Resource.Id.PreviousSpinner));
            var desiredMatches = (int) FindViewById<Spinner>(Resource.Id.MaximumMatches).SelectedItem;
            var desiredWindows = (int) FindViewById<Spinner>(Resource.Id.MaximumWindows).SelectedItem;
            
            var times = new List<string>();
            if (FindViewById<CheckBox>(Resource.Id.chkZeroToEight).Checked)
                times.Add("0");
            if (FindViewById<CheckBox>(Resource.Id.chkEightToSixteen).Checked)
                times.Add("8");
            if (FindViewById<CheckBox>(Resource.Id.chkSixteenToTwentyFour).Checked)
                times.Add("16");

            var parameters = new WeatherParameters
            {
                Zone = zone,
                DesiredWeather = desiredWeather,
                DesiredPreviousWeather = previousWeather,
                DesiredTimes = times,
                MaxTries = desiredWindows,
                MaxMatches = desiredMatches
            };
            var results = WeatherService.GetUpcomingWeatherResults(parameters);

            var weatherTable = FindViewById<TableLayout>(Resource.Id.resultsTable);
            weatherTable.RemoveAllViews();

            var headerRow = BuildRow("Previous\t", "Current\t", "Eorzea\t", "Local\t");
            weatherTable.AddView(headerRow);
            
            foreach (var result in results)
            {
                var row = BuildWeatherRow(
                    result.PreviousWeather, result.CurrentWeather,
                    $"{result.StartTime}", $"{result.TimeOfWeather.ToLocalTime()}");
                weatherTable.AddView(row);
            }

            weatherTable.RequestLayout();
        }

        /// <summary>
        /// Get the list of weather selected for a multi select spinner
        /// </summary>
        private List<string> GetSelectedWeather(MultiSelectionSpinner spinner)
        {
            var list = spinner.GetSelectedStrings();
            if (list.Any(x => x.ToLower() == "any"))
                list.Clear();
            return list;
        }

        /// <summary>
        /// Build a table row to with only text views
        /// </summary>
        private TableRow BuildRow(string col1, string col2, string col3, string col4)
        {
            var tableRow = new TableRow(this);
            var prev = new TextView(this);
            var cur = new TextView(this);
            var et = new TextView(this);
            var lt = new TextView(this);
            prev.Text = col1;
            cur.Text = col2;
            et.Text = col3;
            lt.Text = col4;
            tableRow.AddView(prev);
            tableRow.AddView(cur);
            tableRow.AddView(et);
            tableRow.AddView(lt);
            return tableRow;
        }

        /// <summary>
        /// Build a table row with weather icons and text
        /// </summary>
        private TableRow BuildWeatherRow(string previous, string current, string eorzeaHour, string localTime)
        {
            var tableRow = new TableRow(this);
            var prev = new ImageView(this);
            var cur = new ImageView(this);
            var et = new TextView(this);
            var lt = new TextView(this);
            prev.SetImageResource(GetWeatherIconIdFromName(previous));
            prev.TooltipText = previous;
            cur.SetImageResource(GetWeatherIconIdFromName(current));
            cur.TooltipText = current;
            et.Text = FormatEorzeaHour(eorzeaHour);
            lt.Text = localTime;

            tableRow.AddView(prev);
            tableRow.AddView(cur);
            tableRow.AddView(et);
            tableRow.AddView(lt);
            return tableRow;
        }

        /// <summary>
        /// Get the resource id for the appropriate weather icon
        /// </summary>
        private static int GetWeatherIconIdFromName(string str)
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
                    return 0;
            }
        }

        /// <summary>
        /// Format the eorzean hour to appear as XX:XX
        /// </summary>
        private static string FormatEorzeaHour(string hour)
        {
            if (hour.Length == 1)
                hour = "0" + hour;
            return $"{hour}:00";
        }

        /// <summary>
        /// Handle the user selecting a zone to populate the views for desired weather conditions
        /// </summary>
        private void ZoneSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            var spinner = (Spinner)sender;
            var item = spinner.GetItemAtPosition(args.Position);

            var weatherOptions = new List<string>();
            if (args.Position != 0)
            {
                weatherOptions = WeatherService.GetWeatherOptionsForZone(item.ToString());
                weatherOptions.Insert(0, "Any");
            }

            var desiredElement = FindViewById<MultiSelectionSpinner>(Resource.Id.DesiredSpinner);
            var previousElement = FindViewById<MultiSelectionSpinner>(Resource.Id.PreviousSpinner);

            desiredElement.Items = weatherOptions;
            previousElement.Items = weatherOptions;

            desiredElement.RequestLayout();
            previousElement.RequestLayout();
        }
    }
}

