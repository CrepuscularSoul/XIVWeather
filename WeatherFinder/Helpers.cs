using Android.Content;
using Android.Views;
using Android.Widget;

namespace WeatherFinder
{
    public static class Helpers
    {
        /// <summary>
        /// Get the resource id for the appropriate weather icon
        /// </summary>
        public static int GetWeatherIconIdFromName(string str)
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
                    return default;
            }
        }

        /// <summary>
        /// Format the eorzean hour to appear as XX:XX
        /// </summary>
        public static string FormatEorzeaHour(string hour)
        {
            if (hour.Length == 1)
                hour = "0" + hour;
            return $"{hour}:00";
        }

        public static TextView BuildTextView(
            string text, Context viewContext,
            GravityFlags gravity = GravityFlags.Top | GravityFlags.Start,
            int? minHeight = null, int? minWidth = null,
            (int left, int top, int right, int bottom) pad = default)
        {
            var tv = new TextView(viewContext) { Text = text, Gravity = gravity};
            if (minHeight != null)
                tv.SetMinHeight(minHeight.Value);
            if (minWidth != null)
                tv.SetMinWidth(minWidth.Value);
            tv.SetPadding(pad.left, pad.top, pad.right, pad.bottom);

            return tv;
        }

        public static ImageView BuildImageView(
            string resourceName, Context viewContext, string toolTip,
            int? minHeight = null, int? minWidth = null,
            (int left, int top, int right, int bottom) pad = default)
        {
            var iv = new ImageView(viewContext);
            iv.SetImageResource(GetWeatherIconIdFromName(resourceName));
            iv.TooltipText = toolTip;
            if (minHeight != null)
                iv.SetMinimumHeight(minHeight.Value);
            if (minWidth != null)
                iv.SetMinimumWidth(minWidth.Value);
            iv.SetPadding(pad.left, pad.top, pad.right, pad.bottom);

            return iv;
        }
    }
}