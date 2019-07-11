using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Services;

namespace WeatherFinder
{
    public class WeatherFinderFragment : Fragment
    {
        private View _view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.mainLayout, container, false);
            ConfigureViews(_view);
            return _view;
        }

        /// <summary>
        /// Configure the default data and events for views
        /// </summary>
        private void ConfigureViews(View view)
        {
            var zones = WeatherService.GetZones();
            zones.Insert(0, "Select...");
            var zoneAdapter = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, zones);
            var zoneElement = view.FindViewById<Spinner>(Resource.Id.ZoneSpinner);
            zoneElement.Adapter = zoneAdapter;
            zoneElement.ItemSelected += ZoneSelected;

            var matchOptions = new[] {10, 20, 30, 40, 50};
            var matchAdapter = new ArrayAdapter<int>(view.Context, Android.Resource.Layout.SimpleListItem1, matchOptions);
            var matchElement = view.FindViewById<Spinner>(Resource.Id.MaximumMatches);
            matchElement.Adapter = matchAdapter;

            var windowsOptions = new[] {1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000};
            var windowsAdapter = new ArrayAdapter<int>(view.Context, Android.Resource.Layout.SimpleListItem1, windowsOptions);
            var windowsElement = view.FindViewById<Spinner>(Resource.Id.MaximumWindows);
            windowsElement.Adapter = windowsAdapter;

            var btn = view.FindViewById<Button>(Resource.Id.findWeather);
            btn.Click += FindWeather;
        }

        /// <summary>
        /// Handle the user selecting a zone to populate the views for desired weather conditions
        /// </summary>
        public void ZoneSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            var spinner = (Spinner)sender;
            var item = spinner.GetItemAtPosition(args.Position);

            var weatherOptions = new List<string>();
            if (args.Position != 0)
            {
                weatherOptions = WeatherService.GetWeatherOptionsForZone(item.ToString());
                weatherOptions.Insert(0, "Any");
            }

            var desiredElement = _view.FindViewById<MultiSelectionSpinner>(Resource.Id.DesiredSpinner);
            var previousElement = _view.FindViewById<MultiSelectionSpinner>(Resource.Id.PreviousSpinner);
            var weatherTable = _view.FindViewById<TableLayout>(Resource.Id.resultsTable);

            desiredElement.Items = weatherOptions;
            previousElement.Items = weatherOptions;
            weatherTable.RemoveAllViews();

            desiredElement.RequestLayout();
            previousElement.RequestLayout();
            weatherTable.RequestLayout();
        }

        /// <summary>
        /// Handle the find weather click event and get the data for the user.
        /// </summary>
        private void FindWeather(object sender, EventArgs args)
        {
            var zone = _view.FindViewById<Spinner>(Resource.Id.ZoneSpinner).SelectedItem.ToString();
            var desiredWeather = GetSelectedWeather(_view.FindViewById<MultiSelectionSpinner>(Resource.Id.DesiredSpinner));
            var previousWeather = GetSelectedWeather(_view.FindViewById<MultiSelectionSpinner>(Resource.Id.PreviousSpinner));
            var desiredMatches = (int) _view.FindViewById<Spinner>(Resource.Id.MaximumMatches).SelectedItem;
            var desiredWindows = (int) _view.FindViewById<Spinner>(Resource.Id.MaximumWindows).SelectedItem;

            var times = new List<string>();
            if (_view.FindViewById<CheckBox>(Resource.Id.chkZeroToEight).Checked)
                times.Add("0");
            if (_view.FindViewById<CheckBox>(Resource.Id.chkEightToSixteen).Checked)
                times.Add("8");
            if (_view.FindViewById<CheckBox>(Resource.Id.chkSixteenToTwentyFour).Checked)
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

            var weatherTable = _view.FindViewById<TableLayout>(Resource.Id.resultsTable);
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
            var tableRow = new TableRow(_view.Context);
            var prev = new TextView(_view.Context);
            var cur = new TextView(_view.Context);
            var et = new TextView(_view.Context);
            var lt = new TextView(_view.Context);
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
            var tableRow = new TableRow(_view.Context);
            var prev = new ImageView(_view.Context);
            var cur = new ImageView(_view.Context);
            var et = new TextView(_view.Context);
            var lt = new TextView(_view.Context);
            prev.SetImageResource(Helpers.GetWeatherIconIdFromName(previous));
            prev.TooltipText = previous;
            cur.SetImageResource(Helpers.GetWeatherIconIdFromName(current));
            cur.TooltipText = current;
            et.Text = Helpers.FormatEorzeaHour(eorzeaHour);
            lt.Text = localTime;

            tableRow.AddView(prev);
            tableRow.AddView(cur);
            tableRow.AddView(et);
            tableRow.AddView(lt);
            return tableRow;
        }
    }
}