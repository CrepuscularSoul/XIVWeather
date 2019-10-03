using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using WeatherApp.Domain.Models.Weather;
using WeatherApp.Domain.Services;

namespace WeatherFinder
{
    public class WeatherForecastFragment : Fragment
    {
        private View _view;
        private int _imageHeight;
        private int _imageWidth;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.forecast_layout, container, false);
            ConfigureViews(_view);
            return _view;
        }

        /// <summary>
        /// Configure the region spinner and calculate the height and width to use for forecast panel
        /// </summary>
        private void ConfigureViews(View view)
        {
            var regions = WeatherService.GetRegions();
            regions.Insert(0, "Select...");
            var regionAdapter =
                new ArrayAdapter<string>(view.Context, Android.Resource.Layout.SimpleListItem1, regions);
            var regionElement = view.FindViewById<Spinner>(Resource.Id.RegionSpinner);
            regionElement.Adapter = regionAdapter;
            regionElement.ItemSelected += RegionSelected;

            var dimensions = new BitmapFactory.Options() { InJustDecodeBounds = true };
            BitmapFactory.DecodeResource(_view.Resources, Resource.Drawable.Blizzards, dimensions);
            _imageHeight = (int) (dimensions.OutHeight * ((decimal)dimensions.InTargetDensity / dimensions.InDensity));
            _imageWidth = (int) (dimensions.OutWidth * ((decimal) dimensions.InTargetDensity / dimensions.InDensity));

        }

        /// <summary>
        /// When the user selects a new region get the forecast for those zones
        /// </summary>
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

        /// <summary>
        /// Build the forecast panel based on the data supplied
        /// </summary>
        private LinearLayout BuildForecastView(RegionForecast forecast)
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

        /// <summary>
        /// Builds the zone labels for the weather forecast
        /// </summary>
        private LinearLayout BuildLabelView(RegionForecast forecast)
        {
            var layout = new LinearLayout(_view.Context) { Orientation = Orientation.Vertical };
            var spacer = new TextView(_view.Context);
            spacer.SetMinHeight(_imageHeight);
            layout.AddView(spacer);

            forecast.ZoneForecasts
                .OrderBy(x => x.ZoneOrder)
                .Select(x => Helpers.BuildTextView(x.Zone, _view.Context, GravityFlags.Center, _imageHeight, _imageWidth))
                .ToList()
                .ForEach(x => layout.AddView(x));
            
            return layout;
        }

        /// <summary>
        /// Builds the scrollable forecast section
        /// </summary>
        private HorizontalScrollView BuildForecastPanel(RegionForecast forecast)
        {
            var scrollView = new HorizontalScrollView(_view.Context);
            if (forecast.ZoneForecasts.FirstOrDefault() == null)
                return scrollView;
            
            var forecastLayout = new LinearLayout(_view.Context) { Orientation = Orientation.Vertical };
            forecastLayout.AddView(BuildHeader(forecast));
            forecast.ZoneForecasts
                .OrderBy(x => x.ZoneOrder)
                .Select(BuildSingleZoneForecast)
                .ToList()
                .ForEach(x => forecastLayout.AddView(x));
            
            scrollView.AddView(forecastLayout);
            return scrollView;
        }

        /// <summary>
        /// Builds the header containing the Eorzea Hour each window starts.
        /// </summary>
        private LinearLayout BuildHeader(RegionForecast forecast)
        {
            var layout = new LinearLayout(_view.Context) { Orientation = Orientation.Horizontal};
            forecast.ZoneForecasts.First().WeatherResults.OrderBy(x => x.TimeOfWeather)
                .Select(x => Helpers.BuildTextView(x.StartTime, _view.Context, GravityFlags.Center, _imageHeight, _imageWidth))
                .ToList()
                .ForEach(x => layout.AddView(x));

            return layout;
        }

        /// <summary>
        /// Builds the forecast for a single zone.
        /// </summary>
        private LinearLayout BuildSingleZoneForecast(ZoneForecast zone)
        {
            var layout = new LinearLayout(_view.Context) { Orientation = Orientation.Horizontal};
            zone.WeatherResults
                .OrderBy(x => x.TimeOfWeather)
                .Select(x => BuildImageView(x, zone.Zone))
                .ToList()
                .ForEach(x => layout.AddView(x));
            return layout;
        }

        /// <summary>
        /// Builds the image view for the weather window.
        /// </summary>
        private ImageView BuildImageView(WeatherResult result, string zone)
        {
            var tt = $"{zone}\n{result.CurrentWeather} at {result.TimeOfWeather.ToLocalTime().ToShortTimeString()}";
            return Helpers.BuildImageView(result.CurrentWeather, _view.Context, tt, _imageHeight, _imageWidth);
        }
    }
}