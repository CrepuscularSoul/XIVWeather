using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Android.Graphics;
using WeatherApp.Domain.Models;
using Weather = WeatherApp.Domain.Models.Constants.Weather;
using Zones = WeatherApp.Domain.Models.Constants.Zones;
using Regions = WeatherApp.Domain.Models.Constants.Regions;

namespace WeatherApp.Domain.Services
{
    public class WeatherService
    {
        public static List<WeatherChance> WeatherChances { get; }
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        private const int SecondsPerEorzeaHour = 175,
                          EorzeaHoursPerWeatherWindow = 8,
                          HoursPerDay = 24,
                          MinutesPerHour = 60;

        static WeatherService()
        {
            WeatherChances = GetWeatherChances();
        }

        /// <summary>
        /// Given a zone and desired weather/pre conditions, find up to the specified
        /// number of occurrences within the indicated number of windows.
        /// </summary>
        public static List<WeatherResult> GetUpcomingWeatherResults(
            WeatherParameters parameters)
        {
            var date = DateTime.UtcNow;
            var weatherStart = GetWeatherTimeFloor(date);
            var weatherStartHour = GetEorzeaHour(weatherStart);

            var weather = GetWeatherNameForTime(weatherStart, parameters.Zone);
            var previousWeather = GetWeatherNameForTime(weatherStart.AddSeconds((EorzeaHoursPerWeatherWindow * SecondsPerEorzeaHour) * -1.0), parameters.Zone);

            return GetResults(parameters, weather, previousWeather,
                weatherStartHour, weatherStart);
        }

        /// <summary>
        /// Function handling the loop to get the requested number of weather results
        /// </summary>
        private static List<WeatherResult> GetResults(
            WeatherParameters parameters, string weather, string previousWeather, 
            int weatherStartHour, DateTime weatherStart)
        {
            var tries = 0;
            var matches = 0;
            var results = new List<WeatherResult>();

            while (tries < parameters.MaxTries && matches < parameters.MaxMatches)
            {
                var weatherMatch = !parameters.DesiredWeather.Any()
                                   || parameters.DesiredWeather.Any(x => x == weather);
                var previousWeatherMatch = !parameters.DesiredPreviousWeather.Any() 
                                           || parameters.DesiredPreviousWeather.Any(x => x == previousWeather);
                var timeMatch = !parameters.DesiredTimes.Any() 
                                || parameters.DesiredTimes.Any(x => weatherStartHour.ToString() == x);

                if (weatherMatch && previousWeatherMatch && timeMatch)
                {
                    results.Add(new WeatherResult
                    {
                        Zone = parameters.Zone,
                        CurrentWeather = weather,
                        PreviousWeather = previousWeather, 
                        StartTime = weatherStartHour.ToString(),
                        TimeOfWeather = weatherStart
                    });
                    matches++;
                }

                weatherStart = weatherStart.AddSeconds(EorzeaHoursPerWeatherWindow * SecondsPerEorzeaHour);
                weatherStartHour = GetEorzeaHour(weatherStart);
                previousWeather = weather;
                weather = GetWeatherNameForTime(weatherStart, parameters.Zone);
                tries++;
            }

            return results;
        }

        /// <summary>
        /// Given a DateTime, returns the time as a string formatted as 'hh:mm'
        /// </summary>
        public static string GetEorzeaTime(DateTime date)
        {
            var unixSeconds = GetUnixSeconds(date);
            var eorzeaTimeDouble = unixSeconds / SecondsPerEorzeaHour;
            var hour = (int) Math.Floor(eorzeaTimeDouble % HoursPerDay);
            var minute = (int) (Math.Round((eorzeaTimeDouble - Math.Truncate(eorzeaTimeDouble)), 4) * MinutesPerHour);
            return $"{(hour < 10 ? $"0{hour}" : hour.ToString())}:{(minute < 10 ? $"0{minute}" : minute.ToString())}";
        }

        /// <summary>
        /// Given a date and zone name, get the weather at that time.
        /// </summary>
        public static string GetWeatherNameForTime(DateTime date, string zone)
        {
            return WeatherChances.First(x => x.ZoneName == zone).GetWeather(CalculateForecastTarget(date));
        }

        /// <summary>
        /// Get the eorzean hour for the specified date
        /// </summary>
        public static int GetEorzeaHour(DateTime date)
        {
            var unixSeconds = GetUnixSeconds(date);
            var eorzeaHour = (unixSeconds / SecondsPerEorzeaHour) % HoursPerDay;
            return (int) Math.Floor(eorzeaHour);
        }

        /// <summary>
        /// Based on the specified date, determine the time the current weather window began
        /// </summary>
        public static DateTime GetWeatherTimeFloor(DateTime date)
        {
            var unixSeconds = GetUnixSeconds(date);
            // Get Eorzea hour for weather start
            var eorzeaHour = (unixSeconds / SecondsPerEorzeaHour) % HoursPerDay;
            var startEorzeaHour = eorzeaHour - (eorzeaHour % EorzeaHoursPerWeatherWindow);
            var startUnixSeconds = unixSeconds - (SecondsPerEorzeaHour * (eorzeaHour - startEorzeaHour));
            return GetDateFromSeconds(startUnixSeconds);
        }

        /// <summary>
        /// Calculate the weather chance factor for a given date, which can then be used  to
        /// determine what the weather will be for a specified zone.
        /// </summary>
        public static ulong CalculateForecastTarget(DateTime date) { 
            // Thanks to Rogueadyn's SaintCoinach library for this calculation.
            var unixSeconds = GetUnixSeconds(date);
            var eorzeaHoursSinceUnixEpoch = unixSeconds / SecondsPerEorzeaHour;

            // This is done because the calculations consider 16:00 = 0, 00:00 = 8 and 08:00 = 16
            var increment = (eorzeaHoursSinceUnixEpoch + EorzeaHoursPerWeatherWindow - (eorzeaHoursSinceUnixEpoch % EorzeaHoursPerWeatherWindow)) % HoursPerDay;

            // Calculate the chance value to use for determining weather 
            var totalEorzeanDays = Math.Floor(unixSeconds / (HoursPerDay * SecondsPerEorzeaHour));
            var calcBase = totalEorzeanDays * 100 + increment;
            var step1 = ((uint)calcBase << 11) ^ (uint)calcBase;
            var step2 = (step1 >> 8) ^ step1;
            var chance = step2 % 100;

            return chance;
        }

        /// <summary>
        /// Get the number of seconds since the Unix epoch
        /// </summary>
        private static double GetUnixSeconds(DateTime date)
        {
            var ts = date.Subtract(UnixEpoch);
            return ts.TotalSeconds;
        }

        /// <summary>
        /// Get the date that is <see cref="seconds"/> after the Unix epoch
        /// </summary>
        private static DateTime GetDateFromSeconds(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds).Add(TimeSpan.FromTicks(UnixEpoch.Ticks));
            return new DateTime(ts.Ticks);
        }

        /// <summary>
        /// Get the list of potential weather chances by zone
        /// </summary>
        private static List<WeatherChance> GetWeatherChances()
        {
            return new List<WeatherChance>
            {
                new WeatherChance(Zones.LimsaLominsa, chance =>
                {
                    if (chance < 20) return Weather.Clouds;
                    if (chance < 50) return Weather.ClearSkies;
                    if (chance < 80) return Weather.FairSkies;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.MiddleLaNoscea, chance =>
                {
                    if (chance < 20) return Weather.Clouds;
                    if (chance < 50) return Weather.ClearSkies;
                    if (chance < 70) return Weather.FairSkies;
                    if (chance < 80) return Weather.Wind;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.LowerLaNoscea, chance =>
                {
                    if (chance < 20) return Weather.Clouds;
                    if (chance < 50) return Weather.ClearSkies;
                    if (chance < 70) return Weather.FairSkies;
                    if (chance < 80) return Weather.Wind;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.EasternLaNoscea, chance =>
                {
                    if (chance < 5) return Weather.Fog;
                    if (chance < 50) return Weather.ClearSkies;
                    if (chance < 80) return Weather.FairSkies;
                    if (chance < 90) return Weather.Clouds;
                    if (chance < 95) return Weather.Rain;
                    return Weather.Showers;
                }),
                new WeatherChance(Zones.WesternLaNoscea, chance =>
                {
                    if (chance < 10) return Weather.Fog;
                    if (chance < 40) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 80) return Weather.Clouds;
                    if (chance < 90) return Weather.Wind;
                    return Weather.Gales;
                }),
                new WeatherChance(Zones.UpperLaNoscea, chance =>
                {
                    if (chance < 30) return Weather.ClearSkies;
                    if (chance < 50) return Weather.FairSkies;
                    if (chance < 70) return Weather.Clouds;
                    if (chance < 80) return Weather.Fog;
                    if (chance < 90) return Weather.Thunder;
                    return Weather.Thunderstorms;
                }),
                new WeatherChance(Zones.OuterLaNoscea, chance =>
                {
                    if (chance < 30) return Weather.ClearSkies;
                    if (chance < 50) return Weather.FairSkies;
                    if (chance < 70) return Weather.Clouds;
                    if (chance < 85) return Weather.Fog;
                    return Weather.Rain; 
                }),
                new WeatherChance(Zones.Mist, chance =>
                {
                    if (chance < 20) return Weather.Clouds;
                    if (chance < 50) return Weather.ClearSkies;
                    //NOTE - This double of fair is how it was in the source JS file
                    //if (chance < 70) return Weather.FairSkies;
                    if (chance < 80) return Weather.FairSkies;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.Gridania, chance =>
                {
                    //NOTE - This double of Rain is how it was in the source JS file
                    //if (chance < 5) return Weather.Rain;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 55) return Weather.FairSkies;
                    if (chance < 85) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.CentralShroud, chance =>
                {
                    if (chance < 5) return Weather.Thunder;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 55) return Weather.FairSkies;
                    if (chance < 85) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.EastShroud, chance =>
                {
                    if (chance < 5) return Weather.Thunder;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 55) return Weather.FairSkies;
                    if (chance < 85) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.SouthShroud, chance =>
                {
                    if (chance < 5) return Weather.Fog;
                    if (chance < 10) return Weather.Thunderstorms;
                    if (chance < 25) return Weather.Thunder;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 70) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.NorthShroud, chance =>
                {
                    if (chance < 5) return Weather.Fog;
                    if (chance < 10) return Weather.Showers;
                    if (chance < 25) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 70) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.LavenderBeds, chance =>
                {
                    if (chance < 5) return Weather.Clouds;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 55) return Weather.FairSkies;
                    if (chance < 85) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.Uldah, chance =>
                {
                    if (chance < 40) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 85) return Weather.Clouds;
                    if (chance < 95) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.WesternThanalan, chance =>
                {
                    if (chance < 40) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 85) return Weather.Clouds;
                    if (chance < 95) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.CentralThanalan, chance =>
                {
                    if (chance < 15) return Weather.DustStorms;
                    if (chance < 55) return Weather.ClearSkies;
                    if (chance < 75) return Weather.FairSkies;
                    if (chance < 85) return Weather.Clouds;
                    if (chance < 95) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.EasternThanalan, chance =>
                {
                    if (chance < 40) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 70) return Weather.Clouds;
                    if (chance < 80) return Weather.Fog;
                    if (chance < 85) return Weather.Rain;
                    return Weather.Showers;
                }),
                new WeatherChance(Zones.SouthernThanalan, chance =>
                {
                    if (chance < 20) return Weather.HeatWaves;
                    if (chance < 60) return Weather.ClearSkies;
                    if (chance < 80) return Weather.FairSkies;
                    if (chance < 90) return Weather.Clouds;
                    return Weather.Fog;
                }),
                new WeatherChance(Zones.NorthernThanalan, chance =>
                {
                    if (chance < 5) return Weather.ClearSkies;
                    if (chance < 20) return Weather.FairSkies;
                    if (chance < 50) return Weather.Clouds;
                    return Weather.Fog;
                }),
                new WeatherChance(Zones.Goblet, chance =>
                {
                    if (chance < 40) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 85) return Weather.Clouds;
                    if (chance < 95) return Weather.Fog;
                    return Weather.Rain;
                }),
                new WeatherChance(Zones.MorDhona, chance =>
                {
                    if (chance < 15) return Weather.Clouds;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 60) return Weather.Gloom;
                    if (chance < 75) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.Ishgard, chance =>
                {
                    if (chance < 60) return Weather.Snow;
                    if (chance < 70) return Weather.FairSkies;
                    if (chance < 75) return Weather.ClearSkies;
                    if (chance < 90) return Weather.Clouds;
                    return Weather.Fog;
                }),
                new WeatherChance(Zones.CoerthasCentralHighlands, chance =>
                {
                    if (chance < 20) return Weather.Blizzards;
                    if (chance < 60) return Weather.Snow;
                    if (chance < 70) return Weather.FairSkies;
                    if (chance < 75) return Weather.ClearSkies;
                    if (chance < 90) return Weather.Clouds;
                    return Weather.Fog;
                }),
                new WeatherChance(Zones.CoerthasWesternHighlands, chance =>
                {
                    if (chance < 20) return Weather.Blizzards;
                    if (chance < 60) return Weather.Snow;
                    if (chance < 70) return Weather.FairSkies;
                    if (chance < 75) return Weather.ClearSkies;
                    if (chance < 90) return Weather.Clouds;
                    return Weather.Fog;
                }),
                new WeatherChance(Zones.SeaOfClouds, chance =>
                {
                    if (chance < 30) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 70) return Weather.Clouds;
                    if (chance < 80) return Weather.Fog;
                    if (chance < 90) return Weather.Wind;
                    return Weather.UmbralWind;
                }),
                new WeatherChance(Zones.AzysLla, chance =>
                {
                    if (chance < 35) return Weather.FairSkies;
                    if (chance < 70) return Weather.Clouds;
                    return Weather.Thunder;
                }),
                new WeatherChance(Zones.DravanianForelands, chance =>
                {
                    if (chance < 10) return Weather.Clouds;
                    if (chance < 20) return Weather.Fog;
                    if (chance < 30) return Weather.Thunder;
                    if (chance < 40) return Weather.DustStorms;
                    if (chance < 70) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.DravanianHinterlands, chance =>
                {
                    if (chance < 10) return Weather.Clouds;
                    if (chance < 20) return Weather.Fog;
                    if (chance < 30) return Weather.Rain;
                    if (chance < 40) return Weather.Showers;
                    if (chance < 70) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.ChurningMists, chance =>
                {
                    if (chance < 10) return Weather.Clouds;
                    if (chance < 20) return Weather.Gales;
                    if (chance < 40) return Weather.UmbralStatic;
                    if (chance < 70) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.Idyllshire, chance =>
                {
                    if (chance < 10) return Weather.Clouds;
                    if (chance < 20) return Weather.Fog;
                    if (chance < 30) return Weather.Rain;
                    if (chance < 40) return Weather.Showers;
                    if (chance < 70) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.RhalgrsReach, chance =>
                {
                    if (chance < 15) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 80) return Weather.Clouds;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Thunder;
                }),
                new WeatherChance(Zones.Fringes, chance =>
                {
                    if (chance < 15) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 80) return Weather.Clouds;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Thunder;
                }),
                new WeatherChance(Zones.Peaks, chance =>
                {
                    if (chance < 10) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 75) return Weather.Clouds;
                    if (chance < 85) return Weather.Fog;
                    if (chance < 95) return Weather.Wind;
                    return Weather.DustStorms;
                }),
                new WeatherChance(Zones.Lochs, chance =>
                {
                    if (chance < 20) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 80) return Weather.Clouds;
                    if (chance < 90) return Weather.Fog;
                    return Weather.Thunderstorms;
                }),
                new WeatherChance(Zones.Kugane, chance =>
                {
                    if (chance < 10) return Weather.Rain;
                    if (chance < 20) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 80) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.RubySea, chance =>
                {
                    if (chance < 10) return Weather.Thunder;
                    if (chance < 20) return Weather.Wind;
                    if (chance < 35) return Weather.Clouds;
                    if (chance < 75) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.Yanxia, chance =>
                {
                    if (chance < 5) return Weather.Showers;
                    if (chance < 15) return Weather.Rain;
                    if (chance < 25) return Weather.Fog;
                    if (chance < 40) return Weather.Clouds;
                    if (chance < 80) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.AzimSteppe, chance =>
                {
                    if (chance < 5) return Weather.Gales;
                    if (chance < 10) return Weather.Wind;
                    if (chance < 17) return Weather.Rain;
                    if (chance < 25) return Weather.Fog;
                    if (chance < 35) return Weather.Clouds;
                    if (chance < 75) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.EurekaAnemos, chance =>
                {
                    if (chance < 30) return Weather.FairSkies;
                    if (chance < 60) return Weather.Gales;
                    if (chance < 90) return Weather.Showers;
                    return Weather.Snow;
                }),
                new WeatherChance(Zones.EurekaPagos, chance =>
                {
                    if (chance < 10) return Weather.ClearSkies;
                    if (chance < 28) return Weather.Fog;
                    if (chance < 46) return Weather.HeatWaves;
                    if (chance < 64) return Weather.Snow;
                    if (chance < 82) return Weather.Thunder;
                    return Weather.Blizzards;
                }),
                new WeatherChance(Zones.EurekaPyros, chance =>
                {
                    if (chance < 10) return Weather.FairSkies;
                    if (chance < 28) return Weather.HeatWaves;
                    if (chance < 46) return Weather.Thunder;
                    if (chance < 64) return Weather.Blizzards;
                    if (chance < 82) return Weather.UmbralWind;
                    return Weather.Snow;
                }),
                new WeatherChance(Zones.EurekaHydatos, chance =>
                {
                    if (chance < 12) return Weather.FairSkies;
                    if (chance < 34) return Weather.Showers;
                    if (chance < 56) return Weather.Gloom;
                    if (chance < 78) return Weather.Thunderstorms;
                    return Weather.Snow;
                }),
                new WeatherChance(Zones.Crystarium, chance =>
                {
                    if (chance < 20) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 75) return Weather.Clouds;
                    if (chance < 85) return Weather.Fog;
                    if (chance < 95) return Weather.Rain;
                    return Weather.Thunderstorms;
                }),
                new WeatherChance(Zones.Eulmore, chance =>
                {
                    if (chance < 10) return Weather.Gales;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.Fog;
                    if (chance < 45) return Weather.Clouds;
                    if (chance < 85) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.Lakeland, chance =>
                {
                    if (chance < 20) return Weather.ClearSkies;
                    if (chance < 60) return Weather.FairSkies;
                    if (chance < 75) return Weather.Clouds;
                    if (chance < 85) return Weather.Fog;
                    if (chance < 95) return Weather.Rain;
                    return Weather.Thunderstorms;
                }),
                new WeatherChance(Zones.Kholusia, chance =>
                {
                    if (chance < 10) return Weather.Gales;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 35) return Weather.Fog;
                    if (chance < 45) return Weather.Clouds;
                    if (chance < 85) return Weather.FairSkies;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.AmhAraeng, chance =>
                {
                    if (chance < 45) return Weather.FairSkies;
                    if (chance < 60) return Weather.Clouds;
                    if (chance < 70) return Weather.DustStorms;
                    if (chance < 80) return Weather.HeatWaves;
                    return Weather.ClearSkies;
                }),
                new WeatherChance(Zones.IlMheg, chance =>
                {
                    if (chance < 10) return Weather.Rain;
                    if (chance < 20) return Weather.Fog;
                    if (chance < 35) return Weather.Clouds;
                    if (chance < 45) return Weather.Thunderstorms;
                    if (chance < 60) return Weather.ClearSkies;
                    return Weather.FairSkies;
                }),
                new WeatherChance(Zones.RaktikaGreatwood, chance =>
                {
                    if (chance < 10) return Weather.Fog;
                    if (chance < 20) return Weather.Rain;
                    if (chance < 30) return Weather.UmbralWind;
                    if (chance < 45) return Weather.ClearSkies;
                    if (chance < 85) return Weather.FairSkies;
                    return Weather.Clouds;
                })
            };
        }

        /// <summary>
        /// Get the available weather types for the specified zone
        /// </summary>
        public static List<string> GetWeatherOptionsForZone(string zone)
        {
            switch (zone)
            {
                case Zones.LimsaLominsa:
                    return new List<string> {Weather.Clouds,Weather.ClearSkies,Weather.FairSkies,Weather.Fog,Weather.Rain};
                case Zones.MiddleLaNoscea:
                    return new List<string> {Weather.Clouds,Weather.ClearSkies,Weather.FairSkies,Weather.Wind,Weather.Fog,Weather.Rain};
                case Zones.LowerLaNoscea:
                    return new List<string> {Weather.Clouds,Weather.ClearSkies,Weather.FairSkies,Weather.Wind,Weather.Fog,Weather.Rain};
                case Zones.EasternLaNoscea:
                    return new List<string> {Weather.Fog,Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Rain,Weather.Showers};
                case Zones.WesternLaNoscea:
                    return new List<string> {Weather.Fog,Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Wind,Weather.Gales};
                case Zones.UpperLaNoscea:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Thunder,Weather.Thunderstorms};
                case Zones.OuterLaNoscea:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain };
                case Zones.Mist:
                    return new List<string> {Weather.Clouds,Weather.ClearSkies,Weather.FairSkies,Weather.Fog,Weather.Rain };
                case Zones.Gridania:
                    return new List<string> {Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.CentralShroud:
                    return new List<string> {Weather.Thunder,Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.EastShroud:
                    return new List<string> {Weather.Thunder,Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.SouthShroud:
                    return new List<string> {Weather.Fog,Weather.Thunderstorms,Weather.Thunder,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.NorthShroud:
                    return new List<string> {Weather.Fog,Weather.Showers,Weather.Rain,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.LavenderBeds:
                    return new List<string> {Weather.Clouds,Weather.Rain,Weather.Fog,Weather.FairSkies,Weather.ClearSkies};
                case Zones.Uldah:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain};
                case Zones.WesternThanalan:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain};
                case Zones.CentralThanalan:
                    return new List<string> {Weather.DustStorms,Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain};
                case Zones.EasternThanalan:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain,Weather.Showers};
                case Zones.SouthernThanalan:
                    return new List<string> {Weather.HeatWaves,Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog};
                case Zones.NorthernThanalan:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog};
                case Zones.Goblet:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Rain};
                case Zones.MorDhona:
                    return new List<string> {Weather.Clouds, Weather.Fog, Weather.Gloom, Weather.ClearSkies, Weather.FairSkies};
                case Zones.Ishgard:
                    return new List<string> {Weather.Snow, Weather.FairSkies, Weather.ClearSkies, Weather.Clouds, Weather.Fog};
                case Zones.CoerthasCentralHighlands:
                    return new List<string> {Weather.Blizzards, Weather.Snow, Weather.FairSkies, Weather.ClearSkies, Weather.Clouds, Weather.Fog};
                case Zones.CoerthasWesternHighlands:
                    return new List<string> {Weather.Blizzards, Weather.Snow, Weather.FairSkies, Weather.ClearSkies, Weather.Clouds, Weather.Fog};
                case Zones.SeaOfClouds:
                    return new List<string> {Weather.ClearSkies, Weather.FairSkies, Weather.Clouds, Weather.Fog, Weather.Wind, Weather.UmbralWind};
                case Zones.AzysLla:
                    return new List<string> {Weather.FairSkies, Weather.Clouds, Weather.Thunder};
                case Zones.DravanianForelands:
                    return new List<string> {Weather.Clouds, Weather.Fog, Weather.Thunder, Weather.DustStorms, Weather.ClearSkies, Weather.FairSkies};
                case Zones.DravanianHinterlands:
                    return new List<string> {Weather.Clouds, Weather.Fog, Weather.Rain, Weather.Showers, Weather.ClearSkies, Weather.FairSkies};
                case Zones.ChurningMists:
                    return new List<string> {Weather.Clouds, Weather.Gales, Weather.UmbralStatic, Weather.ClearSkies, Weather.FairSkies};
                case Zones.Idyllshire:
                    return new List<string> {Weather.Clouds, Weather.Fog, Weather.Rain, Weather.Showers, Weather.ClearSkies, Weather.FairSkies};
                case Zones.RhalgrsReach:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Thunder};
                case Zones.Fringes:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Thunder};
                case Zones.Peaks:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Wind,Weather.DustStorms};
                case Zones.Lochs:
                    return new List<string> {Weather.ClearSkies,Weather.FairSkies,Weather.Clouds,Weather.Fog,Weather.Thunderstorms};
                case Zones.Kugane:
                    return new List<string> {Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.RubySea:
                    return new List<string> {Weather.Thunder,Weather.Wind,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.Yanxia:
                    return new List<string> {Weather.Showers,Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.AzimSteppe:
                    return new List<string> {Weather.Gales,Weather.Wind,Weather.Rain,Weather.Fog,Weather.Clouds,Weather.FairSkies,Weather.ClearSkies};
                case Zones.EurekaAnemos:
                    return new List<string> {Weather.FairSkies, Weather.Gales, Weather.Showers, Weather.Snow};
                case Zones.EurekaPagos:
                    return new List<string> { Weather.ClearSkies, Weather.Fog, Weather.HeatWaves, Weather.Snow, Weather.Thunder, Weather.Blizzards };
                case Zones.EurekaPyros:
                    return new List<string> { Weather.FairSkies, Weather.HeatWaves, Weather.Thunder, Weather.Blizzards, Weather.UmbralWind, Weather.Snow };
                case Zones.EurekaHydatos:
                    return new List<string> { Weather.FairSkies, Weather.Showers, Weather.Gloom, Weather.Thunderstorms, Weather.Snow };
                case Zones.Crystarium:
                    return new List<string> { Weather.ClearSkies, Weather.FairSkies, Weather.Clouds, Weather.Fog, Weather.Rain, Weather.Thunderstorms };
                case Zones.Eulmore:
                    return new List<string> { Weather.Gales, Weather.Rain, Weather.Fog, Weather.Clouds, Weather.FairSkies, Weather.ClearSkies };
                case Zones.Lakeland:
                    return new List<string> { Weather.ClearSkies, Weather.FairSkies, Weather.Clouds, Weather.Fog, Weather.Rain, Weather.Thunderstorms };
                case Zones.Kholusia:
                    return new List<string> { Weather.Gales, Weather.Rain, Weather.Fog, Weather.Clouds, Weather.FairSkies, Weather.ClearSkies };
                case Zones.AmhAraeng:
                    return new List<string> { Weather.FairSkies, Weather.Clouds, Weather.DustStorms, Weather.HeatWaves, Weather.ClearSkies };
                case Zones.IlMheg:
                    return new List<string> { Weather.Rain, Weather.Fog, Weather.Clouds, Weather.Thunderstorms, Weather.ClearSkies, Weather.FairSkies };
                case Zones.RaktikaGreatwood:
                    return new List<string> { Weather.Fog, Weather.Rain, Weather.UmbralWind, Weather.ClearSkies, Weather.FairSkies, Weather.Clouds };
                default:
                    throw new InvalidOperationException($"Zone {zone} is not yet implemented.");
            }

        }

        /// <summary>
        /// Get the list of available zones
        /// </summary>
        public static List<string> GetZones()
        {
            return new List<string>
            {
                Zones.LimsaLominsa,
                Zones.MiddleLaNoscea,
                Zones.LowerLaNoscea,
                Zones.EasternLaNoscea,
                Zones.WesternLaNoscea,
                Zones.UpperLaNoscea,
                Zones.OuterLaNoscea,
                Zones.Mist,
                Zones.Gridania,
                Zones.CentralShroud,
                Zones.EastShroud,
                Zones.SouthShroud,
                Zones.NorthShroud,
                Zones.LavenderBeds,
                Zones.Uldah,
                Zones.WesternThanalan,
                Zones.CentralThanalan,
                Zones.EasternThanalan,
                Zones.SouthernThanalan,
                Zones.NorthernThanalan,
                Zones.Goblet,
                Zones.MorDhona,
                Zones.Ishgard,
                Zones.CoerthasCentralHighlands,
                Zones.CoerthasWesternHighlands,
                Zones.SeaOfClouds,
                Zones.AzysLla,
                Zones.DravanianForelands,
                Zones.DravanianHinterlands,
                Zones.ChurningMists,
                Zones.Idyllshire,
                Zones.RhalgrsReach,
                Zones.Fringes,
                Zones.Peaks,
                Zones.Lochs,
                Zones.RubySea,
                Zones.Yanxia,
                Zones.AzimSteppe,
                Zones.Kugane,
                Zones.EurekaAnemos,
                Zones.EurekaPagos,
                Zones.EurekaPyros,
                Zones.EurekaHydatos,
                Zones.Crystarium,
                Zones.Eulmore,
                Zones.Lakeland,
                Zones.Kholusia,
                Zones.AmhAraeng,
                Zones.IlMheg,
                Zones.RaktikaGreatwood
            };
        }

        /// <summary>
        /// Returns a list of available regions
        /// </summary>
        public static List<string> GetRegions()
        {
            return new List<string>
            {
                Regions.BlackShroud,
                Regions.LaNoscea,
                Regions.Thanalan,
                Regions.Ishgard,
                Regions.GyrAbania,
                Regions.FarEast,
                Regions.Norvrandt,
                Regions.Others
            };
        }

        /// <summary>
        /// Get the list of zones that belong to the specified region
        /// </summary>
        private static List<string> GetZonesForRegion(string region)
        {
            var l = new List<string>();
            switch (region)
            {
                case Regions.BlackShroud:
                    l.AddRange(new[] { Zones.Gridania, Zones.CentralShroud, Zones.EastShroud, Zones.SouthShroud, Zones.NorthShroud, Zones.LavenderBeds });
                    break;
                case Regions.LaNoscea:
                    l.AddRange(new[] { Zones.LimsaLominsa, Zones.MiddleLaNoscea, Zones.LowerLaNoscea, Zones.EasternLaNoscea, Zones.WesternThanalan, Zones.UpperLaNoscea, Zones.OuterLaNoscea, Zones.Mist });
                    break;
                case Regions.Thanalan:
                    l.AddRange(new[] { Zones.Uldah, Zones.WesternThanalan, Zones.CentralThanalan, Zones.EasternThanalan, Zones.SouthernThanalan, Zones.NorthernThanalan, Zones.Goblet });
                    break;
                case Regions.Ishgard:
                    l.AddRange(new[] { Zones.Ishgard, Zones.CoerthasCentralHighlands, Zones.CoerthasWesternHighlands, Zones.SeaOfClouds, Zones.AzysLla, Zones.DravanianForelands, Zones.DravanianHinterlands, Zones.ChurningMists, Zones.Idyllshire });
                    break;
                case Regions.GyrAbania:
                    l.AddRange(new[] { Zones.RhalgrsReach, Zones.Fringes, Zones.Peaks, Zones.Lochs });
                    break;
                case Regions.FarEast:
                    l.AddRange(new[] { Zones.RubySea, Zones.Yanxia, Zones.AzimSteppe, Zones.Kugane });
                    break;
                case Regions.Norvrandt:
                    l.AddRange(new[] { Zones.Crystarium, Zones.Eulmore, Zones.Lakeland, Zones.Kholusia, Zones.AmhAraeng, Zones.IlMheg, Zones.RaktikaGreatwood });
                    break;
                case Regions.Others:
                    l.AddRange(new[] { Zones.MorDhona, Zones.EurekaAnemos, Zones.EurekaPagos, Zones.EurekaPyros, Zones.EurekaHydatos });
                    break;
            }

            return l;
        }

        /// <summary>
        /// returns a value for the order that the item should appear when in a list with similar items
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        private static int GetOrderForZone(string zone)
        {
            switch (zone)
            {
                    case Zones.LimsaLominsa:
                        return 1;
                    case Zones.MiddleLaNoscea:
                        return 2;
                    case Zones.LowerLaNoscea:
                        return 3;
                    case Zones.EasternLaNoscea:
                        return 4;
                    case Zones.WesternLaNoscea:
                        return 5;
                    case Zones.UpperLaNoscea:
                        return 6;
                    case Zones.OuterLaNoscea:
                        return 7;
                    case Zones.Mist:
                        return 8;
                    case Zones.Gridania:
                        return 9;
                    case Zones.CentralShroud:
                        return 10;
                    case Zones.EastShroud:
                        return 11;
                    case Zones.SouthShroud:
                        return 12;
                    case Zones.NorthShroud:
                        return 13;
                    case Zones.LavenderBeds:
                        return 14;
                    case Zones.Uldah:
                        return 15;
                    case Zones.WesternThanalan:
                        return 16;
                    case Zones.CentralThanalan:
                        return 17;
                    case Zones.EasternThanalan:
                        return 18;
                    case Zones.SouthernThanalan:
                        return 19;
                    case Zones.NorthernThanalan:
                        return 20;
                    case Zones.Goblet:
                        return 21;
                    case Zones.MorDhona:
                        return 22;
                    case Zones.Ishgard:
                        return 23;
                    case Zones.CoerthasCentralHighlands:
                        return 24;
                    case Zones.CoerthasWesternHighlands:
                        return 25;
                    case Zones.SeaOfClouds:
                        return 26;
                    case Zones.AzysLla:
                        return 27;
                    case Zones.Idyllshire:
                        return 28;
                    case Zones.DravanianForelands:
                        return 29;
                    case Zones.DravanianHinterlands:
                        return 30;
                    case Zones.ChurningMists:
                        return 31;
                    case Zones.RhalgrsReach:
                        return 32;
                    case Zones.Fringes:
                        return 33;
                    case Zones.Peaks:
                        return 34;
                    case Zones.Lochs:
                        return 35;
                    case Zones.RubySea:
                        return 36;
                    case Zones.Yanxia:
                        return 37;
                    case Zones.AzimSteppe:
                        return 38;
                    case Zones.Kugane:
                        return 39;
                    case Zones.EurekaAnemos:
                        return 40;
                    case Zones.EurekaPagos:
                        return 41;
                    case Zones.EurekaPyros:
                        return 42;
                    case Zones.EurekaHydatos:
                        return 43;
                    case Zones.Crystarium:
                        return 44;
                    case Zones.Eulmore:
                        return 45;
                    case Zones.Lakeland:
                        return 46;
                    case Zones.Kholusia:
                        return 47;
                    case Zones.AmhAraeng:
                        return 48;
                    case Zones.IlMheg:
                        return 49;
                    case Zones.RaktikaGreatwood:
                        return 50;
                    default:
                        return default;
            }
        }

        /// <summary>
        /// Get the upcoming weather forecast for the specified region
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public static RegionForecast GetWeatherForecastForRegion(string region)
        {
            var forecast = new RegionForecast(region);
            var zones = GetZonesForRegion(region);
            var parameters = new WeatherParameters
            {
                MaxMatches = 25,
                MaxTries = 25
            };
            foreach (var zone in zones)
            {
                parameters.Zone = zone;
                var order = GetOrderForZone(zone);
                var results = GetUpcomingWeatherResults(parameters);
                forecast.AddZoneForecast(zone, order, results);
            }

            return forecast;
        }

    }
}