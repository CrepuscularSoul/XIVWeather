using System;
using System.Collections.Generic;
using System.Linq;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Locations;
using WeatherApp.Domain.Models.Weather;
using WeatherApp.Domain.Repository;
using Zones = WeatherApp.Domain.Models.Constants.Zones;

namespace WeatherApp.Domain.Services
{
    public class WeatherService
    {
        public static List<Models.Locations.Region> Regions { get; private set; }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        private const int SecondsPerEorzeaHour = 175,
                          EorzeaHoursPerWeatherWindow = 8,
                          HoursPerDay = 24,
                          MinutesPerHour = 60;

        static WeatherService()
        {
            Regions = RegionRepository.GetRegions();
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

            return GetResults(parameters, weather, previousWeather, weatherStartHour, weatherStart);
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
                if (ParametersMatchWindow(parameters, weather, previousWeather, weatherStartHour))
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
        public static string GetEorzeaTimeOfDay(DateTime date)
        {
            var us = GetUnixSeconds(date);
            var et = GetElapsedEorzeaTime(us);
            var hour = GetHourFromEorzeaTime(et);
            var minute = GetMinuteFromEorzeaTime(et);
            return $"{FormatTimePart(hour)}:{FormatTimePart(minute)}";
        }

        /// <summary>
        /// Given a date and zone name, get the weather at that time.
        /// </summary>
        public static string GetWeatherNameForTime(DateTime date, string zone)
        {
            var weatherEnum = Regions.GetZone(zone).GetWeatherForCalculatedChance(CalculateForecastTarget(date));
            return Enums.WeatherMapping[weatherEnum];
        }

        /// <summary>
        /// Get the eorzean hour for the specified date
        /// </summary>
        public static int GetEorzeaHour(DateTime date)
        {
            var unixSeconds = GetUnixSeconds(date);
            return GetHourFromEorzeaTime(GetElapsedEorzeaTime(unixSeconds));
        }

        /// <summary>
        /// Based on the specified date, determine the time the current weather window began
        /// </summary>
        public static DateTime GetWeatherTimeFloor(DateTime date)
        {
            var unixSeconds = GetUnixSeconds(date);
            var eorzeaHour = GetHourFromEorzeaTime(GetElapsedEorzeaTime(unixSeconds));
            var startEorzeaHour = GetWeatherStartHour(eorzeaHour);
            var startUnixSeconds = GetWeatherStartUnixSeconds(unixSeconds, eorzeaHour, startEorzeaHour);
            return GetDateFromSeconds(startUnixSeconds);
        }

        /// <summary>
        /// Calculate the weather chance factor for a given date, which can then be used  to
        /// determine what the weather will be for a specified zone.
        /// </summary>
        public static ulong CalculateForecastTarget(DateTime date) { 
            // Thanks to Rogueadyn's SaintCoinach library for this calculation.
            var unixSeconds = GetUnixSeconds(date);
            var eorzeaTime = GetElapsedEorzeaTime(unixSeconds);

            // This is done because the calculations consider 16:00 = 0, 00:00 = 8 and 08:00 = 16
            var increment = (eorzeaTime + EorzeaHoursPerWeatherWindow - (eorzeaTime % EorzeaHoursPerWeatherWindow)) % HoursPerDay;

            // Calculate the chance value to use for determining weather 
            var totalEorzeanDays = Math.Floor(unixSeconds / (HoursPerDay * SecondsPerEorzeaHour));
            var calcBase = totalEorzeanDays * 100 + increment;
            var step1 = ((uint)calcBase << 11) ^ (uint)calcBase;
            var step2 = (step1 >> 8) ^ step1;
            var chance = step2 % 100;

            return chance;
        }

        /// <summary>
        /// Get the available weather types for the specified zone
        /// </summary>
        public static List<string> GetWeatherOptionsForZone(string zone) =>
            Regions
                .FirstOrDefault(x => x.Zones.Any(y => y.Name == zone))
                ?.Zones
                .First(x => x.Name == zone)
                .WeatherConditions
                .Select(x => Enums.WeatherMapping[x])
                .ToList()
            ?? new List<string>();

        /// <summary>
        /// Get the list of available zones
        /// </summary>
        public static List<string> GetZones() =>
            Regions.SelectMany(x => x.Zones.Select(y => y.Name)).ToList();

        /// <summary>
        /// Returns a list of available regions
        /// </summary>
        public static List<string> GetRegions() =>
            Regions.Select(x => x.Name).ToList();

        /// <summary>
        /// Get the list of zones that belong to the specified region
        /// </summary>
        public static List<string> GetZonesForRegion(string region) =>
            Regions
                .FirstOrDefault(x => x.Name == region)?
                .Zones
                .Select(x => x.Name)
                .ToList()
            ?? new List<string>();

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

        #region Helper Functions

        /// <summary>
        /// Checks whether the desired conditions in parameters match the window being tested.
        /// </summary>
        private static bool ParametersMatchWindow(
            WeatherParameters parameters, string weather, 
            string previousWeather, int weatherStartHour)
        {
            var weatherMatch = !parameters.DesiredWeather.Any()
                               || parameters.DesiredWeather.Any(x => x == weather);
            var previousWeatherMatch = !parameters.DesiredPreviousWeather.Any()
                                       || parameters.DesiredPreviousWeather.Any(x => x == previousWeather);
            var timeMatch = !parameters.DesiredTimes.Any()
                            || parameters.DesiredTimes.Any(x => weatherStartHour.ToString() == x);

            return weatherMatch && previousWeatherMatch && timeMatch;
        }

        /// <summary>
        /// Get the number of seconds since the Unix epoch
        /// </summary>
        private static double GetUnixSeconds(DateTime date) 
            => date.Subtract(UnixEpoch).TotalSeconds;

        /// <summary>
        /// Get the date that is <see cref="seconds"/> after the Unix epoch
        /// </summary>
        private static DateTime GetDateFromSeconds(double seconds) 
            => new DateTime(TimeSpan.FromSeconds(seconds).Add(TimeSpan.FromTicks(UnixEpoch.Ticks)).Ticks);

        /// <summary>
        /// Given the total seconds since the Unix Epoch, returns the number
        /// of total elapsed Eorzea time.
        /// </summary>
        private static double GetElapsedEorzeaTime(double secondsSinceEpoch) 
            => secondsSinceEpoch / SecondsPerEorzeaHour;

        /// <summary>
        /// Given the total time elapsed in Eorzea, returns the current hour in the day.
        /// </summary>
        private static int GetHourFromEorzeaTime(double et) 
            => (int) Math.Floor(et % HoursPerDay);

        /// <summary>
        /// Given the total time elapsed in Eorzea, returns the current minute of the
        /// current hour in the day.
        /// </summary>
        private static int GetMinuteFromEorzeaTime(double et) 
            => (int) (Math.Round(et - Math.Truncate(et), 4) * MinutesPerHour);

        /// <summary>
        /// Takes a number representing an hour or minute, and pads it with a 0 or the
        /// supplied pad value if it is less than time so that 2 minutes past 8 displays
        /// as '08:02' instead of '8:2'
        /// </summary>
        private static string FormatTimePart(int value, string padValue = "0") 
            => value < 10 ? $"{padValue}{value}" : value.ToString();

        /// <summary>
        /// Given the current Eorzean hour of the day, returns the hour the current weather window began.
        /// </summary>
        private static int GetWeatherStartHour(int currentEorzeaHour)
            => currentEorzeaHour - (currentEorzeaHour % EorzeaHoursPerWeatherWindow);

        /// <summary>
        /// Given the seconds since the unix epoch, the current Eorzean hour of the day,
        /// and the start hour of the current weather window, calculates the seconds since epoch
        /// that teh current weather window began.
        /// </summary>
        private static double GetWeatherStartUnixSeconds(double unixSeconds, int currentHour, int startHour)
            => unixSeconds - (SecondsPerEorzeaHour * (currentHour - startHour));

        /// <summary>
        /// returns a value for the order that the item should appear when in a list with similar items
        /// </summary>
        private static int GetOrderForZone(string zone)
        {
            return zone switch
            {
                Zones.LimsaLominsa => 1,
                Zones.MiddleLaNoscea => 2,
                Zones.LowerLaNoscea => 3,
                Zones.EasternLaNoscea => 4,
                Zones.WesternLaNoscea => 5,
                Zones.UpperLaNoscea => 6,
                Zones.OuterLaNoscea => 7,
                Zones.Mist => 8,
                Zones.Gridania => 9,
                Zones.CentralShroud => 10,
                Zones.EastShroud => 11,
                Zones.SouthShroud => 12,
                Zones.NorthShroud => 13,
                Zones.LavenderBeds => 14,
                Zones.Uldah => 15,
                Zones.WesternThanalan => 16,
                Zones.CentralThanalan => 17,
                Zones.EasternThanalan => 18,
                Zones.SouthernThanalan => 19,
                Zones.NorthernThanalan => 20,
                Zones.Goblet => 21,
                Zones.MorDhona => 22,
                Zones.Ishgard => 23,
                Zones.CoerthasCentralHighlands => 24,
                Zones.CoerthasWesternHighlands => 25,
                Zones.SeaOfClouds => 26,
                Zones.AzysLla => 27,
                Zones.Idyllshire => 28,
                Zones.DravanianForelands => 29,
                Zones.DravanianHinterlands => 30,
                Zones.ChurningMists => 31,
                Zones.RhalgrsReach => 32,
                Zones.Fringes => 33,
                Zones.Peaks => 34,
                Zones.Lochs => 35,
                Zones.RubySea => 36,
                Zones.Yanxia => 37,
                Zones.AzimSteppe => 38,
                Zones.Kugane => 39,
                Zones.EurekaAnemos => 40,
                Zones.EurekaPagos => 41,
                Zones.EurekaPyros => 42,
                Zones.EurekaHydatos => 43,
                Zones.Crystarium => 44,
                Zones.Eulmore => 45,
                Zones.Lakeland => 46,
                Zones.Kholusia => 47,
                Zones.AmhAraeng => 48,
                Zones.IlMheg => 49,
                Zones.RaktikaGreatwood => 50,
                _ => default
            };
        }
        #endregion
    }
}