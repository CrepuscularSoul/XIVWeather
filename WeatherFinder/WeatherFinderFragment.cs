using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain.Models.Weather;
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
            ConfigureSpinners(view);
            ConfigureButtons(view);
        }

        /// <summary>
        /// Configures the buttons in the view.
        /// </summary>
        private void ConfigureButtons(View view)
        {
            var btn = view.FindViewById<Button>(Resource.Id.findWeather);
            btn.Click += FindWeather;
        }

        /// <summary>
        /// Configures the spinners in the view.
        /// </summary>
        private void ConfigureSpinners(View view)
        {
            ConfigureZoneSpinner(view);
            ConfigureMatchSpinner(view);
            ConfigureWindowLookAheadSpinner(view);
        }

        /// <summary>
        /// Configures the maximum window look ahead spinner.
        /// </summary>
        private static void ConfigureWindowLookAheadSpinner(View view)
        {
            var windowsOptions = new[] {1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000};
            var windowsAdapter =
                new ArrayAdapter<int>(view.Context, Android.Resource.Layout.SimpleListItem1, windowsOptions);
            var windowsElement = view.FindViewById<Spinner>(Resource.Id.MaximumWindows);
            windowsElement.Adapter = windowsAdapter;
        }

        /// <summary>
        /// Configures the maximum number of matches spinner.
        /// </summary>
        private static void ConfigureMatchSpinner(View view)
        {
            var matchOptions = new[] {10, 20, 30, 40, 50};
            var matchAdapter =
                new ArrayAdapter<int>(view.Context, Android.Resource.Layout.SimpleListItem1, matchOptions);
            var matchElement = view.FindViewById<Spinner>(Resource.Id.MaximumMatches);
            matchElement.Adapter = matchAdapter;
        }

        /// <summary>
        /// Configures the zone spinner.
        /// </summary>
        private void ConfigureZoneSpinner(View view)
        {
            var zones = WeatherService.GetZones();
            zones.Insert(0, "Select...");
            var zoneAdapter = new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, zones);
            var zoneElement = view.FindViewById<Spinner>(Resource.Id.ZoneSpinner);
            zoneElement.Adapter = zoneAdapter;
            zoneElement.ItemSelected += ZoneSelected;
        }

        /// <summary>
        /// Handle the user selecting a zone to populate the views for desired weather conditions
        /// </summary>
        public void ZoneSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            var spinner = (Spinner) sender;
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
            var results = GetWeatherResults();
            BuildWeatherTable(results);
        }

        /// <summary>
        /// Gets the weather results to display.
        /// </summary>
        private List<WeatherResult> GetWeatherResults()
        {
            var zone = _view.FindViewById<Spinner>(Resource.Id.ZoneSpinner).SelectedItem.ToString();
            var dWeather = GetSelectedWeather(_view.FindViewById<MultiSelectionSpinner>(Resource.Id.DesiredSpinner));
            var pWeather = GetSelectedWeather(_view.FindViewById<MultiSelectionSpinner>(Resource.Id.PreviousSpinner));
            var dMatches = (int) _view.FindViewById<Spinner>(Resource.Id.MaximumMatches).SelectedItem;
            var dWindows = (int) _view.FindViewById<Spinner>(Resource.Id.MaximumWindows).SelectedItem;
            var times = GetSelectedTimes();

            var parameters = new WeatherParameters
            {
                Zone = zone,
                DesiredWeather = dWeather,
                DesiredPreviousWeather = pWeather,
                DesiredTimes = times,
                MaxTries = dWindows,
                MaxMatches = dMatches
            };
            
            return WeatherService.GetUpcomingWeatherResults(parameters);
        }

        /// <summary>
        /// Builds the weather table and requests a refresh of the layout.
        /// </summary>
        private void BuildWeatherTable(List<WeatherResult> results)
        {
            var weatherTable = _view.FindViewById<TableLayout>(Resource.Id.resultsTable);
            weatherTable.RemoveAllViews();

            var headerRow = BuildRow("Previous\t", "Current\t", "Eorzea\t", "Local\t");
            weatherTable.AddView(headerRow);

            results.Select(x => BuildWeatherRow(x.PreviousWeather, x.CurrentWeather, x.StartTime, $"{x.TimeOfWeather.ToLocalTime()}"))
                .ToList()
                .ForEach(weatherTable.AddView);
            
            weatherTable.RequestLayout();
        }

        /// <summary>
        /// Gets the selected times.
        /// </summary>
        private List<string> GetSelectedTimes()
        {
            var times = new List<string>();
            if (_view.FindViewById<CheckBox>(Resource.Id.chkZeroToEight).Checked)
                times.Add("0");
            if (_view.FindViewById<CheckBox>(Resource.Id.chkEightToSixteen).Checked)
                times.Add("8");
            if (_view.FindViewById<CheckBox>(Resource.Id.chkSixteenToTwentyFour).Checked)
                times.Add("16");
            return times;
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
        /// Build a table row with only text views
        /// </summary>
        private TableRow BuildRow(params string[] text)
        {
            var tableRow = new TableRow(_view.Context);
            text.Select(x => Helpers.BuildTextView(x, _view.Context, pad: (10, 0, 10, 0)))
                .ToList()
                .ForEach(x => tableRow.AddView(x));
            return tableRow;
        }

        /// <summary>
        /// Build a table row with weather icons and text
        /// </summary>
        private TableRow BuildWeatherRow(string previous, string current, string eorzeaHour, string localTime)
        {
            var tableRow = new TableRow(_view.Context);
            
            var prev = Helpers.BuildImageView(previous, _view.Context, previous, pad: (10, 0, 10, 0));
            var cur = Helpers.BuildImageView(current, _view.Context, current, pad: (10, 0, 10, 0));
            var et = Helpers.BuildTextView(Helpers.FormatEorzeaHour(eorzeaHour), _view.Context, pad: (10, 0, 10, 0));
            var lt = Helpers.BuildTextView(localTime, _view.Context, pad: (10, 0, 10, 0));
            
            tableRow.AddView(prev);
            tableRow.AddView(cur);
            tableRow.AddView(et);
            tableRow.AddView(lt);
            
            return tableRow;
        }
    }
}