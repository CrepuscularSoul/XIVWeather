using System;
using System.Collections.Generic;
using System.Linq;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Locations;
using WeatherApp.Domain.Repository;
using Zones = WeatherApp.Domain.Models.Constants.Zones;

namespace WeatherApp.Domain.Services
{
    public class WeatherService
    {
        public static List<Models.Locations.Region> RegionsOfTheWorld { get; private set; }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        private const int SecondsPerEorzeaHour = 175,
                          EorzeaHoursPerWeatherWindow = 8,
                          HoursPerDay = 24,
                          MinutesPerHour = 60;

        static WeatherService()
        {
            RegionsOfTheWorld = RegionRepository.GetRegions();
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
            var thing = RegionsOfTheWorld.GetZone(zone).GetWeatherForCalculatedChance(CalculateForecastTarget(date));
            return Enums.WeatherMapping[thing];
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
        /// Get the available weather types for the specified zone
        /// </summary>
        public static List<string> GetWeatherOptionsForZone(string zone) =>
            RegionsOfTheWorld
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
            RegionsOfTheWorld.SelectMany(x => x.Zones.Select(y => y.Name)).ToList();

        /// <summary>
        /// Returns a list of available regions
        /// </summary>
        public static List<string> GetRegions() =>
            RegionsOfTheWorld.Select(x => x.Name).ToList();

        /// <summary>
        /// Get the list of zones that belong to the specified region
        /// </summary>
        private static List<string> GetZonesForRegion(string region) =>
            RegionsOfTheWorld
                .FirstOrDefault(x => x.Name == region)?
                .Zones
                .Select(x => x.Name)
                .ToList()
            ?? new List<string>();

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