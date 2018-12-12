﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain;

namespace WeatherFinder
{
    public class WeatherForecastFragment : Fragment
    {
        private View _view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.forecast_layout, container, false);
            ConfigureViews(_view);
            return _view;
        }

        private void ConfigureViews(View view)
        {
            var regions = WeatherService.GetRegions();
            regions.Insert(0, "Select...");
            var regionAdapter =
                new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, regions);
            var regionElement = view.FindViewById<Spinner>(Resource.Id.RegionSpinner);
            regionElement.Adapter = regionAdapter;
            regionElement.ItemSelected += RegionSelected;
        }

        public void RegionSelected(object sender, AdapterView.ItemSelectedEventArgs args)
        {
            var spinner = (Spinner) sender;
            var region = spinner.GetItemAtPosition(args.Position).ToString();
            var forecastView = _view.FindViewById<FrameLayout>(Resource.Id.forecastContainer);
            
            forecastView.RemoveAllViews();

            if (region != string.Empty)
            {
                var weatherForecasts = WeatherService.GetWeatherForecastForRegion(region);
                var forecastViewLayout = BuildForecastView(weatherForecasts);
                forecastView.AddView(forecastViewLayout);
            }
            
            forecastView.RequestLayout();
        }

        private LinearLayout BuildForecastView(Dictionary<string, List<WeatherResult>> forecast)
        {
            var labels = BuildLabelView(forecast);
            var forecastPanel = BuildForecastPanel(forecast);

            var layout = new LinearLayout(_view.Context)
            {
                Orientation = Orientation.Horizontal
            };
            layout.AddView(labels);
            layout.AddView(forecastPanel);

            return layout;
        }

        private LinearLayout BuildLabelView(Dictionary<string, List<WeatherResult>> forecast)
        {
            var layout = new LinearLayout(_view.Context) { Orientation = Orientation.Vertical };
            var spacer = new TextView(_view.Context);
            layout.AddView(spacer);

            foreach (var label in forecast.Keys.OrderBy(x => x))
            {
                var tv = new TextView(_view.Context) {Text = label};
                layout.AddView(tv);
            }

            return layout;
        }

        private ScrollView BuildForecastPanel(Dictionary<string, List<WeatherResult>> forecast)
        {
            var scrollView = new ScrollView(_view.Context);
            var outerLinearLayout = new LinearLayout(_view.Context) { Orientation = Orientation.Vertical };

            foreach (var kvp in forecast.OrderBy(x => x.Key))
            {
                var linearLayout = new LinearLayout(_view.Context) { Orientation = Orientation.Horizontal };
                foreach (var value in kvp.Value.OrderBy(x => x.TimeOfWeather))
                {
                    var iv = new ImageView(_view.Context);
                    iv.SetImageResource(Helpers.GetWeatherIconIdFromName(value.CurrentWeather));
                    iv.TooltipText = $"{value.CurrentWeather} at {value.TimeOfWeather.Hour}:{value.TimeOfWeather.Minute}";
                    linearLayout.AddView(iv);
                }
                outerLinearLayout.AddView(linearLayout);
            }

            scrollView.AddView(outerLinearLayout);
            return scrollView;
        }
    }
}