using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using Javax.Crypto.Spec;

namespace WeatherApp.Domain
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
                new WeatherChance("Limsa Lominsa", chance =>
                {
                    if (chance < 20) return "Clouds";
                    if (chance < 50) return "Clear Skies";
                    if (chance < 80) return "Fair Skies";
                    if (chance < 90) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Middle La Noscea", chance =>
                {
                    if (chance < 20) return "Clouds";
                    if (chance < 50) return "Clear Skies";
                    if (chance < 70) return "Fair Skies";
                    if (chance < 80) return "Wind";
                    if (chance < 90) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Lower La Noscea", chance =>
                {
                    if (chance < 20) return "Clouds";
                    if (chance < 50) return "Clear Skies";
                    if (chance < 70) return "Fair Skies";
                    if (chance < 80) return "Wind";
                    if (chance < 90) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Eastern La Noscea", chance =>
                {
                    if (chance < 5) return "Fog";
                    if (chance < 50) return "Clear Skies";
                    if (chance < 80) return "Fair Skies";
                    if (chance < 90) return "Clouds";
                    if (chance < 95) return "Rain";
                    return "Showers";
                }),
                new WeatherChance("Western La Noscea", chance =>
                {
                    if (chance < 10) return "Fog";
                    if (chance < 40) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 80) return "Clouds";
                    if (chance < 90) return "Wind";
                    return "Gales";
                }),
                new WeatherChance("Upper La Noscea", chance =>
                {
                    if (chance < 30) return "Clear Skies";
                    if (chance < 50) return "Fair Skies";
                    if (chance < 70) return "Clouds";
                    if (chance < 80) return "Fog";
                    if (chance < 90) return "Thunder";
                    return "Thunderstorms";
                }),
                new WeatherChance("Outer La Noscea", chance =>
                {
                    if (chance < 30) return "Clear Skies";
                    if (chance < 50) return "Fair Skies";
                    if (chance < 70) return "Clouds";
                    if (chance < 85) return "Fog";
                    return "Rain"; 
                }),
                new WeatherChance("Mist", chance =>
                {
                    if (chance < 20) return "Clouds";
                    if (chance < 50) return "Clear Skies";
                    //NOTE - This double of fair is how it was in the source JS file
                    //if (chance < 70) return "Fair Skies";
                    if (chance < 80) return "Fair Skies";
                    if (chance < 90) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Gridania", chance =>
                {
                    //NOTE - This double of Rain is how it was in the source JS file
                    //if (chance < 5) return "Rain";
                    if (chance < 20) return "Rain";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 55) return "Fair Skies";
                    if (chance < 85) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("Central Shroud", chance =>
                {
                    if (chance < 5) return "Thunder";
                    if (chance < 20) return "Rain";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 55) return "Fair Skies";
                    if (chance < 85) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("East Shroud", chance =>
                {
                    if (chance < 5) return "Thunder";
                    if (chance < 20) return "Rain";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 55) return "Fair Skies";
                    if (chance < 85) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("South Shroud", chance =>
                {
                    if (chance < 5) return "Fog";
                    if (chance < 10) return "Thunderstorms";
                    if (chance < 25) return "Thunder";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 70) return "Fair Skies";
                    return "Clear Skies";
                }),
                new WeatherChance("North Shroud", chance =>
                {
                    if (chance < 5) return "Fog";
                    if (chance < 10) return "Showers";
                    if (chance < 25) return "Rain";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 70) return "Fair Skies";
                    return "Clear Skies";
                }),
                new WeatherChance("The Lavender Beds", chance =>
                {
                    if (chance < 5) return "Clouds";
                    if (chance < 20) return "Rain";
                    if (chance < 30) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 55) return "Fair Skies";
                    if (chance < 85) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("Ul'dah", chance =>
                {
                    if (chance < 40) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 85) return "Clouds";
                    if (chance < 95) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Western Thanalan", chance =>
                {
                    if (chance < 40) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 85) return "Clouds";
                    if (chance < 95) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Central Thanalan", chance =>
                {
                    if (chance < 15) return "Dust Storms";
                    if (chance < 55) return "Clear Skies";
                    if (chance < 75) return "Fair Skies";
                    if (chance < 85) return "Clouds";
                    if (chance < 95) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Eastern Thanalan", chance =>
                {
                    if (chance < 40) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 70) return "Clouds";
                    if (chance < 80) return "Fog";
                    if (chance < 85) return "Rain";
                    return "Showers";
                }),
                new WeatherChance("Southern Thanalan", chance =>
                {
                    if (chance < 20) return "Heat Waves";
                    if (chance < 60) return "Clear Skies";
                    if (chance < 80) return "Fair Skies";
                    if (chance < 90) return "Clouds";
                    return "Fog";
                }),
                new WeatherChance("Northern Thanalan", chance =>
                {
                    if (chance < 5) return "Clear Skies";
                    if (chance < 20) return "Fair Skies";
                    if (chance < 50) return "Clouds";
                    return "Fog";
                }),
                new WeatherChance("The Goblet", chance =>
                {
                    if (chance < 40) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 85) return "Clouds";
                    if (chance < 95) return "Fog";
                    return "Rain";
                }),
                new WeatherChance("Mor Dhona", chance =>
                {
                    if (chance < 15) return "Clouds";
                    if (chance < 30) return "Fog";
                    if (chance < 60) return "Gloom";
                    if (chance < 75) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("Ishgard", chance =>
                {
                    if (chance < 60) return "Snow";
                    if (chance < 70) return "Fair Skies";
                    if (chance < 75) return "Clear Skies";
                    if (chance < 90) return "Clouds";
                    return "Fog";
                }),
                new WeatherChance("Coerthas Central Highlands", chance =>
                {
                    if (chance < 20) return "Blizzards";
                    if (chance < 60) return "Snow";
                    if (chance < 70) return "Fair Skies";
                    if (chance < 75) return "Clear Skies";
                    if (chance < 90) return "Clouds";
                    return "Fog";
                }),
                new WeatherChance("Coerthas Western Highlands", chance =>
                {
                    if (chance < 20) return "Blizzards";
                    if (chance < 60) return "Snow";
                    if (chance < 70) return "Fair Skies";
                    if (chance < 75) return "Clear Skies";
                    if (chance < 90) return "Clouds";
                    return "Fog";
                }),
                new WeatherChance("The Sea of Clouds", chance =>
                {
                    if (chance < 30) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 70) return "Clouds";
                    if (chance < 80) return "Fog";
                    if (chance < 90) return "Wind";
                    return "Umbral Wind";
                }),
                new WeatherChance("Azys Lla", chance =>
                {
                    if (chance < 35) return "Fair Skies";
                    if (chance < 70) return "Clouds";
                    return "Thunder";
                }),
                new WeatherChance("The Dravanian Forelands", chance =>
                {
                    if (chance < 10) return "Clouds";
                    if (chance < 20) return "Fog";
                    if (chance < 30) return "Thunder";
                    if (chance < 40) return "Dust Storms";
                    if (chance < 70) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("The Dravanian Hinterlands", chance =>
                {
                    if (chance < 10) return "Clouds";
                    if (chance < 20) return "Fog";
                    if (chance < 30) return "Rain";
                    if (chance < 40) return "Showers";
                    if (chance < 70) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("The Churning Mists", chance =>
                {
                    if (chance < 10) return "Clouds";
                    if (chance < 20) return "Gales";
                    if (chance < 40) return "Umbral Static";
                    if (chance < 70) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("Idyllshire", chance =>
                {
                    if (chance < 10) return "Clouds";
                    if (chance < 20) return "Fog";
                    if (chance < 30) return "Rain";
                    if (chance < 40) return "Showers";
                    if (chance < 70) return "Clear Skies";
                    return "Fair Skies";
                }),
                new WeatherChance("Rhalgr's Reach", chance =>
                {
                    if (chance < 15) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 80) return "Clouds";
                    if (chance < 90) return "Fog";
                    return "Thunder";
                }),
                new WeatherChance("The Fringes", chance =>
                {
                    if (chance < 15) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 80) return "Clouds";
                    if (chance < 90) return "Fog";
                    return "Thunder";
                }),
                new WeatherChance("The Peaks", chance =>
                {
                    if (chance < 10) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 75) return "Clouds";
                    if (chance < 85) return "Fog";
                    if (chance < 95) return "Wind";
                    return "Dust Storms";
                }),
                new WeatherChance("The Lochs", chance =>
                {
                    if (chance < 20) return "Clear Skies";
                    if (chance < 60) return "Fair Skies";
                    if (chance < 80) return "Clouds";
                    if (chance < 90) return "Fog";
                    return "Thunderstorms";
                }),
                new WeatherChance("Kugane", chance =>
                {
                    if (chance < 10) return "Rain";
                    if (chance < 20) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 80) return "Fair Skies";
                    return "Clear Skies";
                }),
                new WeatherChance("The Ruby Sea", chance =>
                {
                    if (chance < 10) return "Thunder";
                    if (chance < 20) return "Wind";
                    if (chance < 35) return "Clouds";
                    if (chance < 75) return "Fair Skies";
                    return "Clear Skies";
                }),
                new WeatherChance("Yanxia", chance =>
                {
                    if (chance < 5) return "Showers";
                    if (chance < 15) return "Rain";
                    if (chance < 25) return "Fog";
                    if (chance < 40) return "Clouds";
                    if (chance < 80) return "Fair Skies";
                    return "Clear Skies";
                }),
                new WeatherChance("The Azim Steppe", chance =>
                {
                    if (chance < 5) return "Gales"; 
                    if (chance < 10) return "Wind"; 
                    if (chance < 17) return "Rain"; 
                    if (chance < 25) return "Fog"; 
                    if (chance < 35) return "Clouds"; 
                    if (chance < 75) return "Fair Skies"; 
                    return "Clear Skies"; 
                }),
                new WeatherChance("Eureka Anemos", chance =>
                {
                    if (chance < 30) return "Fair Skies";
                    if (chance < 60) return "Gales";
                    if (chance < 90) return "Showers";
                    return "Snow";
                }),
                new WeatherChance("Eureka Pagos", chance =>
                {
                    if (chance < 10) return "Clear Skies";
                    if (chance < 28) return "Fog";
                    if (chance < 46) return "Heat Waves";
                    if (chance < 64) return "Snow";
                    if (chance < 82) return "Thunder";
                    return "Blizzards";
                }),
                new WeatherChance("Eureka Pyros", chance =>
                {
                    if (chance < 10) return "Fair Skies"; 
                    if (chance < 28) return "Heat Waves"; 
                    if (chance < 46) return "Thunder"; 
                    if (chance < 64) return "Blizzards"; 
                    if (chance < 82) return "Umbral Wind"; 
                    return "Snow";
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
                case "Limsa Lominsa": 
                    return new List<string> {"Clouds","Clear Skies","Fair Skies","Fog","Rain"};
                case "Middle La Noscea": 
                    return new List<string> {"Clouds","Clear Skies","Fair Skies","Wind","Fog","Rain"};
                case "Lower La Noscea": 
                    return new List<string> {"Clouds","Clear Skies","Fair Skies","Wind","Fog","Rain"};
                case "Eastern La Noscea": 
                    return new List<string> {"Fog","Clear Skies","Fair Skies","Clouds","Rain","Showers"};
                case "Western La Noscea": 
                    return new List<string> {"Fog","Clear Skies","Fair Skies","Clouds","Wind","Gales"};
                case "Upper La Noscea": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Thunder","Thunderstorms"};
                case "Outer La Noscea": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Rain" };
                case "Mist": 
                    return new List<string> {"Clouds","Clear Skies","Fair Skies","Fog","Rain" };
                case "Gridania": 
                    return new List<string> {"Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "Central Shroud": 
                    return new List<string> {"Thunder","Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "East Shroud": 
                    return new List<string> {"Thunder","Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "South Shroud": 
                    return new List<string> {"Fog","Thunderstorms","Thunder","Clouds","Fair Skies","Clear Skies"};
                case "North Shroud": 
                    return new List<string> {"Fog","Showers","Rain","Clouds","Fair Skies","Clear Skies"};
                case "The Lavender Beds":
                    return new List<string> {"Clouds","Rain","Fog","Fair Skies","Clear Skies"};
                case "Ul'dah": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Rain"};
                case "Western Thanalan": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Rain"};
                case "Central Thanalan": 
                    return new List<string> {"Dust Storms","Clear Skies","Fair Skies","Clouds","Fog","Rain"};
                case "Eastern Thanalan": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Rain","Showers"};
                case "Southern Thanalan": 
                    return new List<string> {"Heat Waves","Clear Skies","Fair Skies","Clouds","Fog"};
                case "Northern Thanalan": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog"};
                case "The Goblet": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Rain"};
                case "Mor Dhona": 
                    return new List<string> {"Clouds", "Fog", "Gloom", "Clear Skies", "Fair Skies"};
                case "Ishgard": 
                    return new List<string> {"Snow", "Fair Skies", "Clear Skies", "Clouds", "Fog"};
                case "Coerthas Central Highlands": 
                    return new List<string> {"Blizzards", "Snow", "Fair Skies", "Clear Skies", "Clouds", "Fog"};
                case "Coerthas Western Highlands": 
                    return new List<string> {"Blizzards", "Snow", "Fair Skies", "Clear Skies", "Clouds", "Fog"};
                case "The Sea of Clouds": 
                    return new List<string> {"Clear Skies", "Fair Skies", "Clouds", "Fog", "Wind", "Umbral Wind"};
                case "Azys Lla": 
                    return new List<string> {"Fair Skies", "Clouds", "Thunder"};
                case "The Dravanian Forelands": 
                    return new List<string> {"Clouds", "Fog", "Thunder", "Dust Storms", "Clear Skies", "Fair Skies"};
                case "The Dravanian Hinterlands": 
                    return new List<string> {"Clouds", "Fog", "Rain", "Showers", "Clear Skies", "Fair Skies"};
                case "The Churning Mists": 
                    return new List<string> {"Clouds", "Gales", "Umbral Static", "Clear Skies", "Fair Skies"};
                case "Idyllshire": 
                    return new List<string> {"Clouds", "Fog", "Rain", "Showers", "Clear Skies", "Fair Skies"};
                case "Rhalgr's Reach": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Thunder"};
                case "The Fringes": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Thunder"};
                case "The Peaks": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Wind","Dust Storms"};
                case "The Lochs": 
                    return new List<string> {"Clear Skies","Fair Skies","Clouds","Fog","Thunderstorms"};
                case "Kugane": 
                    return new List<string> {"Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "The Ruby Sea": 
                    return new List<string> {"Thunder","Wind","Clouds","Fair Skies","Clear Skies"};
                case "Yanxia": 
                    return new List<string> {"Showers","Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "The Azim Steppe": 
                    return new List<string> {"Gales","Wind","Rain","Fog","Clouds","Fair Skies","Clear Skies"};
                case "Eureka Anemos": 
                    return new List<string> {"Fair Skies", "Gales", "Showers", "Snow"};
                case "Eureka Pagos":
                    return new List<string> {"Clear Skies", "Fog", "Heat Waves", "Snow", "Thunder", "Blizzards"};
                case "Eureka Pyros":
                    return new List<string> {"Fair Skies", "Heat Waves", "Thunder", "Blizzards", "Umbral Wind", "Snow"};
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
                "Limsa Lominsa",
                "Middle La Noscea",
                "Lower La Noscea",
                "Eastern La Noscea",
                "Western La Noscea",
                "Upper La Noscea",
                "Outer La Noscea",
                "Mist",
                "Gridania",
                "Central Shroud",
                "East Shroud",
                "South Shroud",
                "North Shroud",
                "The Lavender Beds",
                "Ul'dah",
                "Western Thanalan",
                "Central Thanalan",
                "Eastern Thanalan",
                "Southern Thanalan",
                "Northern Thanalan",
                "The Goblet",
                "Mor Dhona",
                "Ishgard",
                "Coerthas Central Highlands",
                "Coerthas Western Highlands",
                "The Sea of Clouds",
                "Azys Lla",
                "The Dravanian Forelands",
                "The Dravanian Hinterlands",
                "The Churning Mists",
                "Idyllshire",
                "Rhalgr's Reach",
                "The Fringes",
                "The Peaks",
                "The Lochs",
                "The Ruby Sea",
                "Yanxia",
                "The Azim Steppe",
                "Kugane",
                "Eureka Anemos",
                "Eureka Pagos",
                "Eureka Pyros"
            };
        }

        /// <summary>
        /// Returns a list of available regions
        /// </summary>
        public static List<string> GetRegions()
        {
            return new List<string>
            {
                "Black Shroud",
                "La Noscea",
                "Thanalan",
                "Ishgard/Surrounding",
                "Gyr Abania",
                "Far East",
                "Others"
            };
        }

        /// <summary>
        /// Get the list of zones that belong to the specified region
        /// </summary>
        private static List<string> GetZonesForRegion(string region)
        {
            var l = new List<string>();
            switch (region.ToLower())
            {
                case "black shroud":
                    l.AddRange(new [] { "Gridania", "Central Shroud", "East Shroud", "South Shroud", "North Shroud", "The Lavender Beds" });
                    break;
                case "la noscea":
                    l.AddRange(new [] { "Limsa Lominsa", "Middle La Noscea", "Lower La Noscea", "Eastern La Noscea", "Western La Noscea", "Upper La Noscea", "Outer La Noscea", "Mist" });
                    break;
                case "thanalan":
                    l.AddRange(new [] { "Ul'dah", "Western Thanalan", "Central Thanalan", "Eastern Thanalan", "Southern Thanalan", "Northern Thanalan", "The Goblet" });
                    break;
                case "ishgard/surrounding":
                    l.AddRange(new [] { "Ishgard", "Coerthas Central Highlands", "Coerthas Western Highlands", "The Sea of Clouds", "Azys Lla", "The Dravanian Forelands", "The Dravanian Hinterlands", "The Churning Mists", "Idyllshire" });
                    break;
                case "gyr abania":
                    l.AddRange(new [] { "Rhalgr's Reach", "The Fringes", "The Peaks", "The Lochs" });
                    break;
                case "far east":
                    l.AddRange(new [] { "The Ruby Sea", "Yanxia", "The Azim Steppe", "Kugane" });
                    break;
                case "others":
                    l.AddRange(new [] { "Mor Dhona", "Eureka Anemos", "Eureka Pagos", "Eureka Pyros" });
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
                    case "Limsa Lominsa":
                        return 1;
                    case "Middle La Noscea":
                        return 2;
                    case "Lower La Noscea":
                        return 3;
                    case "Eastern La Noscea":
                        return 4;
                    case "Western La Noscea":
                        return 5;
                    case "Upper La Noscea":
                        return 6;
                    case "Outer La Noscea":
                        return 7;
                    case "Mist":
                        return 8;
                    case "Gridania":
                        return 9;
                    case "Central Shroud":
                        return 10;
                    case "East Shroud":
                        return 11;
                    case "South Shroud":
                        return 12;
                    case "North Shroud":
                        return 13;
                    case "The Lavender Beds":
                        return 14;
                    case "Ul'dah":
                        return 15;
                    case "Western Thanalan":
                        return 16;
                    case "Central Thanalan":
                        return 17;
                    case "Eastern Thanalan":
                        return 18;
                    case "Southern Thanalan":
                        return 19;
                    case "Northern Thanalan":
                        return 20;
                    case "The Goblet":
                        return 21;
                    case "Mor Dhona":
                        return 22;
                    case "Ishgard":
                        return 23;
                    case "Coerthas Central Highlands":
                        return 24;
                    case "Coerthas Western Highlands":
                        return 25;
                    case "The Sea of Clouds":
                        return 26;
                    case "Azys Lla":
                        return 27;
                    case "Idyllshire":
                        return 28;
                    case "The Dravanian Forelands":
                        return 29;
                    case "The Dravanian Hinterlands":
                        return 30;
                    case "The Churning Mists":
                        return 31;
                    case "Rhalgr's Reach":
                        return 32;
                    case "The Fringes":
                        return 33;
                    case "The Peaks":
                        return 34;
                    case "The Lochs":
                        return 35;
                    case "The Ruby Sea":
                        return 36;
                    case "Yanxia":
                        return 37;
                    case "The Azim Steppe":
                        return 38;
                    case "Kugane":
                        return 39;
                    case "Eureka Anemos":
                        return 40;
                    case "Eureka Pagos":
                        return 41;
                    case "Eureka Pyros":
                        return 42;
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