﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using WeatherApp.Domain.Models.Weather;
using WeatherApp.Domain.Services;

namespace WeatherApp.Domain.Tests
{
    [TestClass]
    public class WeatherAppTests
    {
        [Test]
        public void GetWeatherWorks()
        {
            var results = WeatherService.GetUpcomingWeatherResults(
                new WeatherParameters
                {
                    DesiredPreviousWeather = new List<string> { },
                    DesiredWeather = new List<string> {"Clear Skies"},
                    Zone = "The Sea of Clouds",
                    DesiredTimes = new List<string> { },
                    MaxMatches = 5,
                    MaxTries = 2000
                });

            results.Should().NotBeNull();
            results.Count.Should().BeGreaterThan(0);
            results.Count.Should().Be(5);
        }

        [Test]
        public void RegionWeatherContainsNoDuplicates()
        {
            var result = WeatherService.Regions.Any(
                x => x.Zones.Any(
                    y => y.WeatherConditions.Distinct().Count() != y.WeatherConditions.Count));

            result.Should().BeFalse();
        }

        [Test]
        public void RegionWeatherBreakpointsContainOneBreakpointForOneHundred()
        {
            var result = WeatherService.Regions.All(
                x => x.Zones.All(
                    y => y.WeatherBreakpoints.Count(z => z.Value == 100) == 1));

            result.Should().BeTrue();
        }

        #region Get Eorzea Time Works

        [TestCase(2018, 12, 28, 13, 43, 19, "07:59")]
        [TestCase(2018, 12, 28, 13, 43, 20, "08:00")]
        [TestCase(2018, 12, 28, 13, 43, 21, "08:00")]
        [TestCase(2018, 12, 28, 13, 43, 22, "08:00")]
        [TestCase(2018, 12, 28, 13, 43, 23, "08:01")]
        [TestCase(2018, 12, 28, 13, 43, 24, "08:01")]
        [TestCase(2018, 12, 28, 13, 43, 25, "08:01")]
        [TestCase(2018, 12, 28, 13, 43, 26, "08:02")]
        [TestCase(2018, 12, 28, 13, 43, 27, "08:02")]
        [TestCase(2018, 12, 28, 13, 43, 28, "08:02")]
        [TestCase(2018, 12, 28, 13, 43, 29, "08:03")]
        [TestCase(2018, 12, 28, 13, 43, 30, "08:03")]
        [TestCase(2018, 12, 28, 13, 43, 31, "08:03")]
        [TestCase(2018, 12, 28, 13, 43, 32, "08:04")]
        [TestCase(2018, 12, 28, 13, 43, 33, "08:04")]
        [TestCase(2018, 12, 28, 13, 43, 34, "08:04")]
        [TestCase(2018, 12, 28, 13, 43, 35, "08:05")]
        [TestCase(2018, 12, 28, 13, 43, 36, "08:05")]
        [TestCase(2018, 12, 28, 13, 43, 37, "08:05")]
        [TestCase(2018, 12, 28, 13, 43, 38, "08:06")]
        [TestCase(2018, 12, 28, 13, 43, 39, "08:06")]
        [TestCase(2018, 12, 28, 13, 43, 40, "08:06")]
        [TestCase(2018, 12, 28, 13, 43, 41, "08:07")]
        [TestCase(2018, 12, 28, 13, 43, 42, "08:07")]
        [TestCase(2018, 12, 28, 13, 43, 43, "08:07")]
        [TestCase(2018, 12, 28, 13, 43, 44, "08:08")]
        [TestCase(2018, 12, 28, 13, 43, 45, "08:08")]
        [TestCase(2018, 12, 28, 13, 43, 46, "08:08")]
        [TestCase(2018, 12, 28, 13, 43, 47, "08:09")]
        [TestCase(2018, 12, 28, 13, 43, 48, "08:09")]
        [TestCase(2018, 12, 28, 13, 43, 49, "08:09")]
        [TestCase(2018, 12, 28, 13, 43, 50, "08:10")]
        [TestCase(2018, 12, 28, 13, 43, 51, "08:10")]
        [TestCase(2018, 12, 28, 13, 43, 52, "08:10")]
        [TestCase(2018, 12, 28, 13, 43, 53, "08:11")]
        [TestCase(2018, 12, 28, 13, 43, 54, "08:11")]
        [TestCase(2018, 12, 28, 13, 43, 55, "08:12")]
        [TestCase(2018, 12, 28, 13, 43, 56, "08:12")]
        [TestCase(2018, 12, 28, 13, 43, 57, "08:12")]
        [TestCase(2018, 12, 28, 13, 43, 58, "08:13")]
        [TestCase(2018, 12, 28, 13, 43, 59, "08:13")]
        [TestCase(2018, 12, 28, 13, 44, 00, "08:13")]
        [TestCase(2018, 12, 28, 13, 44, 01, "08:14")]
        [TestCase(2018, 12, 28, 13, 44, 02, "08:14")]
        [TestCase(2018, 12, 28, 13, 44, 03, "08:14")]
        [TestCase(2018, 12, 28, 13, 44, 04, "08:15")]
        [TestCase(2018, 12, 28, 13, 44, 05, "08:15")]
        [TestCase(2018, 12, 28, 13, 44, 06, "08:15")]
        [TestCase(2018, 12, 28, 13, 44, 07, "08:16")]
        [TestCase(2018, 12, 28, 13, 44, 08, "08:16")]
        [TestCase(2018, 12, 28, 13, 44, 09, "08:16")]
        [TestCase(2018, 12, 28, 13, 44, 10, "08:17")]
        [TestCase(2018, 12, 28, 13, 44, 11, "08:17")]
        [TestCase(2018, 12, 28, 13, 44, 12, "08:17")]
        [TestCase(2018, 12, 28, 13, 44, 13, "08:18")]
        [TestCase(2018, 12, 28, 13, 44, 14, "08:18")]
        [TestCase(2018, 12, 28, 13, 44, 15, "08:18")]
        [TestCase(2018, 12, 28, 13, 44, 16, "08:19")]
        [TestCase(2018, 12, 28, 13, 44, 17, "08:19")]
        [TestCase(2018, 12, 28, 13, 44, 18, "08:19")]
        [TestCase(2018, 12, 28, 13, 44, 19, "08:20")]
        [TestCase(2018, 12, 28, 13, 44, 20, "08:20")]
        [TestCase(2018, 12, 28, 13, 44, 21, "08:20")]
        [TestCase(2018, 12, 28, 13, 44, 22, "08:21")]
        [TestCase(2018, 12, 28, 13, 44, 23, "08:21")]
        [TestCase(2018, 12, 28, 13, 44, 24, "08:21")]
        [TestCase(2018, 12, 28, 13, 44, 25, "08:22")]
        [TestCase(2018, 12, 28, 13, 44, 26, "08:22")]
        [TestCase(2018, 12, 28, 13, 44, 27, "08:22")]
        [TestCase(2018, 12, 28, 13, 44, 28, "08:23")]
        [TestCase(2018, 12, 28, 13, 44, 29, "08:23")]
        [TestCase(2018, 12, 28, 13, 44, 30, "08:24")]
        [TestCase(2018, 12, 28, 13, 44, 31, "08:24")]
        [TestCase(2018, 12, 28, 13, 44, 32, "08:24")]
        [TestCase(2018, 12, 28, 13, 44, 33, "08:25")]
        [TestCase(2018, 12, 28, 13, 44, 34, "08:25")]
        [TestCase(2018, 12, 28, 13, 44, 35, "08:25")]
        [TestCase(2018, 12, 28, 13, 44, 36, "08:26")]
        [TestCase(2018, 12, 28, 13, 44, 37, "08:26")]
        [TestCase(2018, 12, 28, 13, 44, 38, "08:26")]
        [TestCase(2018, 12, 28, 13, 44, 39, "08:27")]
        [TestCase(2018, 12, 28, 13, 44, 40, "08:27")]
        [TestCase(2018, 12, 28, 13, 44, 41, "08:27")]
        [TestCase(2018, 12, 28, 13, 44, 42, "08:28")]
        [TestCase(2018, 12, 28, 13, 44, 43, "08:28")]
        [TestCase(2018, 12, 28, 13, 44, 44, "08:28")]
        [TestCase(2018, 12, 28, 13, 44, 45, "08:29")]
        [TestCase(2018, 12, 28, 13, 44, 46, "08:29")]
        [TestCase(2018, 12, 28, 13, 44, 47, "08:29")]
        [TestCase(2018, 12, 28, 13, 44, 48, "08:30")]
        [TestCase(2018, 12, 28, 13, 44, 49, "08:30")]
        [TestCase(2018, 12, 28, 13, 44, 50, "08:30")]
        [TestCase(2018, 12, 28, 13, 44, 51, "08:31")]
        [TestCase(2018, 12, 28, 13, 44, 52, "08:31")]
        [TestCase(2018, 12, 28, 13, 44, 53, "08:31")]
        [TestCase(2018, 12, 28, 13, 44, 54, "08:32")]
        [TestCase(2018, 12, 28, 13, 44, 55, "08:32")]
        [TestCase(2018, 12, 28, 13, 44, 56, "08:32")]
        [TestCase(2018, 12, 28, 13, 44, 57, "08:33")]
        [TestCase(2018, 12, 28, 13, 44, 58, "08:33")]
        [TestCase(2018, 12, 28, 13, 44, 59, "08:33")]
        [TestCase(2018, 12, 28, 13, 45, 00, "08:34")]
        [TestCase(2018, 12, 28, 13, 45, 01, "08:34")]
        [TestCase(2018, 12, 28, 13, 45, 02, "08:34")]
        [TestCase(2018, 12, 28, 13, 45, 03, "08:35")]
        [TestCase(2018, 12, 28, 13, 45, 04, "08:35")]
        [TestCase(2018, 12, 28, 13, 45, 05, "08:36")]
        [TestCase(2018, 12, 28, 13, 45, 06, "08:36")]
        [TestCase(2018, 12, 28, 13, 45, 07, "08:36")]
        [TestCase(2018, 12, 28, 13, 45, 08, "08:37")]
        [TestCase(2018, 12, 28, 13, 45, 09, "08:37")]
        [TestCase(2018, 12, 28, 13, 45, 10, "08:37")]
        [TestCase(2018, 12, 28, 13, 45, 11, "08:38")]
        [TestCase(2018, 12, 28, 13, 45, 12, "08:38")]
        [TestCase(2018, 12, 28, 13, 45, 13, "08:38")]
        [TestCase(2018, 12, 28, 13, 45, 14, "08:39")]
        [TestCase(2018, 12, 28, 13, 45, 15, "08:39")]
        [TestCase(2018, 12, 28, 13, 45, 16, "08:39")]
        [TestCase(2018, 12, 28, 13, 45, 17, "08:40")]
        [TestCase(2018, 12, 28, 13, 45, 18, "08:40")]
        [TestCase(2018, 12, 28, 13, 45, 19, "08:40")]
        [TestCase(2018, 12, 28, 13, 45, 20, "08:41")]
        [TestCase(2018, 12, 28, 13, 45, 21, "08:41")]
        [TestCase(2018, 12, 28, 13, 45, 22, "08:41")]
        [TestCase(2018, 12, 28, 13, 45, 23, "08:42")]
        [TestCase(2018, 12, 28, 13, 45, 24, "08:42")]
        [TestCase(2018, 12, 28, 13, 45, 25, "08:42")]
        [TestCase(2018, 12, 28, 13, 45, 26, "08:43")]
        [TestCase(2018, 12, 28, 13, 45, 27, "08:43")]
        [TestCase(2018, 12, 28, 13, 45, 28, "08:43")]
        [TestCase(2018, 12, 28, 13, 45, 29, "08:44")]
        [TestCase(2018, 12, 28, 13, 45, 30, "08:44")]
        [TestCase(2018, 12, 28, 13, 45, 31, "08:44")]
        [TestCase(2018, 12, 28, 13, 45, 32, "08:45")]
        [TestCase(2018, 12, 28, 13, 45, 33, "08:45")]
        [TestCase(2018, 12, 28, 13, 45, 34, "08:45")]
        [TestCase(2018, 12, 28, 13, 45, 35, "08:46")]
        [TestCase(2018, 12, 28, 13, 45, 36, "08:46")]
        [TestCase(2018, 12, 28, 13, 45, 37, "08:46")]
        [TestCase(2018, 12, 28, 13, 45, 38, "08:47")]
        [TestCase(2018, 12, 28, 13, 45, 39, "08:47")]
        [TestCase(2018, 12, 28, 13, 45, 40, "08:48")]
        [TestCase(2018, 12, 28, 13, 45, 41, "08:48")]
        [TestCase(2018, 12, 28, 13, 45, 42, "08:48")]
        [TestCase(2018, 12, 28, 13, 45, 43, "08:49")]
        [TestCase(2018, 12, 28, 13, 45, 44, "08:49")]
        [TestCase(2018, 12, 28, 13, 45, 45, "08:49")]
        [TestCase(2018, 12, 28, 13, 45, 46, "08:50")]
        [TestCase(2018, 12, 28, 13, 45, 47, "08:50")]
        [TestCase(2018, 12, 28, 13, 45, 48, "08:50")]
        [TestCase(2018, 12, 28, 13, 45, 49, "08:51")]
        [TestCase(2018, 12, 28, 13, 45, 50, "08:51")]
        [TestCase(2018, 12, 28, 13, 45, 51, "08:51")]
        [TestCase(2018, 12, 28, 13, 45, 52, "08:52")]
        [TestCase(2018, 12, 28, 13, 45, 53, "08:52")]
        [TestCase(2018, 12, 28, 13, 45, 54, "08:52")]
        [TestCase(2018, 12, 28, 13, 45, 55, "08:53")]
        [TestCase(2018, 12, 28, 13, 45, 56, "08:53")]
        [TestCase(2018, 12, 28, 13, 45, 57, "08:53")]
        [TestCase(2018, 12, 28, 13, 45, 58, "08:54")]
        [TestCase(2018, 12, 28, 13, 45, 59, "08:54")]
        [TestCase(2018, 12, 28, 13, 46, 00, "08:54")]
        [TestCase(2018, 12, 28, 13, 46, 01, "08:55")]
        [TestCase(2018, 12, 28, 13, 46, 02, "08:55")]
        [TestCase(2018, 12, 28, 13, 46, 03, "08:55")]
        [TestCase(2018, 12, 28, 13, 46, 04, "08:56")]
        [TestCase(2018, 12, 28, 13, 46, 05, "08:56")]
        [TestCase(2018, 12, 28, 13, 46, 06, "08:56")]
        [TestCase(2018, 12, 28, 13, 46, 07, "08:57")]
        [TestCase(2018, 12, 28, 13, 46, 08, "08:57")]
        [TestCase(2018, 12, 28, 13, 46, 09, "08:57")]
        [TestCase(2018, 12, 28, 13, 46, 10, "08:58")]
        [TestCase(2018, 12, 28, 13, 46, 11, "08:58")]
        [TestCase(2018, 12, 28, 13, 46, 12, "08:58")]
        [TestCase(2018, 12, 28, 13, 46, 13, "08:59")]
        [TestCase(2018, 12, 28, 13, 46, 14, "08:59")]
        [TestCase(2018, 12, 28, 13, 46, 15, "09:00")]
        public void GetEorzeaTimeWorks(int year, int month, int day, int hour, int minute, int second, string expected)
        {
            var result = WeatherService.GetEorzeaTimeOfDay(new DateTime(year, month, day, hour, minute, second));
            result.Should().Be(expected);
        }

        #endregion

        #region Get Weather Name For Time Returns Correct Result For Zone

        [TestCase(2018, 12, 13, 14, 23, 20, "Eureka Pyros", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Eureka Pyros", "Heat Waves")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Eureka Pyros", "Heat Waves")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Eureka Pyros", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Eureka Pyros", "Umbral Wind")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Eureka Pyros", "Blizzards")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Eureka Pyros", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Eureka Pyros", "Umbral Wind")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Eureka Pyros", "Thunder")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Eureka Pyros", "Umbral Wind")]
        [TestCase(2018, 10, 19, 13, 27, 0, "The Azim Steppe", "Wind")]
        [TestCase(2018, 12, 13, 14, 23, 20, "The Azim Steppe", "Gales")]
        [TestCase(2018, 12, 13, 14, 46, 40, "The Azim Steppe", "Clouds")]
        [TestCase(2018, 12, 13, 15, 10, 0, "The Azim Steppe", "Clouds")]
        [TestCase(2018, 12, 13, 15, 33, 20, "The Azim Steppe", "Wind")]
        [TestCase(2018, 12, 13, 15, 56, 40, "The Azim Steppe", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "The Azim Steppe", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "The Azim Steppe", "Wind")]
        [TestCase(2018, 12, 13, 17, 06, 40, "The Azim Steppe", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "The Azim Steppe", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "The Azim Steppe", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "The Lochs", "Clear Skies")]
        [TestCase(2018, 12, 13, 14, 46, 40, "The Lochs", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "The Lochs", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "The Lochs", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 56, 40, "The Lochs", "Clouds")]
        [TestCase(2018, 12, 13, 16, 20, 0, "The Lochs", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "The Lochs", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 06, 40, "The Lochs", "Clouds")]
        [TestCase(2018, 12, 13, 17, 30, 0, "The Lochs", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "The Lochs", "Clouds")]
        [TestCase(2018, 12, 13, 14, 23, 20, "The Sea of Clouds", "Clear Skies")]
        [TestCase(2018, 12, 13, 14, 46, 40, "The Sea of Clouds", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "The Sea of Clouds", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "The Sea of Clouds", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 56, 40, "The Sea of Clouds", "Fog")]
        [TestCase(2018, 12, 13, 16, 20, 0, "The Sea of Clouds", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "The Sea of Clouds", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 06, 40, "The Sea of Clouds", "Fog")]
        [TestCase(2018, 12, 13, 17, 30, 0, "The Sea of Clouds", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "The Sea of Clouds", "Clouds")]
        [TestCase(2018, 12, 13, 14, 23, 20, "South Shroud", "Fog")]
        [TestCase(2018, 12, 13, 14, 46, 40, "South Shroud", "Fog")]
        [TestCase(2018, 12, 13, 15, 10, 0, "South Shroud", "Fog")]
        [TestCase(2018, 12, 13, 15, 33, 20, "South Shroud", "Thunderstorms")]
        [TestCase(2018, 12, 13, 15, 56, 40, "South Shroud", "Clear Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "South Shroud", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "South Shroud", "Thunderstorms")]
        [TestCase(2018, 12, 13, 17, 06, 40, "South Shroud", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "South Shroud", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "South Shroud", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "Southern Thanalan", "Heat Waves")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Southern Thanalan", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Southern Thanalan", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Southern Thanalan", "Heat Waves")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Southern Thanalan", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Southern Thanalan", "Clear Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Southern Thanalan", "Heat Waves")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Southern Thanalan", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Southern Thanalan", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Southern Thanalan", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "Mor Dhona", "Clouds")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Mor Dhona", "Fog")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Mor Dhona", "Fog")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Mor Dhona", "Clouds")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Mor Dhona", "Clear Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Mor Dhona", "Gloom")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Mor Dhona", "Clouds")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Mor Dhona", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Mor Dhona", "Gloom")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Mor Dhona", "Clear Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "Limsa Lominsa", "Clouds")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Limsa Lominsa", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Limsa Lominsa", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Limsa Lominsa", "Clouds")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Limsa Lominsa", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Limsa Lominsa", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Limsa Lominsa", "Clouds")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Limsa Lominsa", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Limsa Lominsa", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Limsa Lominsa", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "The Fringes", "Clear Skies")]
        [TestCase(2018, 12, 13, 14, 46, 40, "The Fringes", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "The Fringes", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "The Fringes", "Clear Skies")]
        [TestCase(2018, 12, 13, 15, 56, 40, "The Fringes", "Clouds")]
        [TestCase(2018, 12, 13, 16, 20, 0, "The Fringes", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "The Fringes", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 06, 40, "The Fringes", "Clouds")]
        [TestCase(2018, 12, 13, 17, 30, 0, "The Fringes", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "The Fringes", "Clouds")]
        [TestCase(2018, 12, 13, 14, 23, 20, "The Lavender Beds", "Clouds")]
        [TestCase(2018, 12, 13, 14, 46, 40, "The Lavender Beds", "Fog")]
        [TestCase(2018, 12, 13, 15, 10, 0, "The Lavender Beds", "Fog")]
        [TestCase(2018, 12, 13, 15, 33, 20, "The Lavender Beds", "Rain")]
        [TestCase(2018, 12, 13, 15, 56, 40, "The Lavender Beds", "Clear Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "The Lavender Beds", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "The Lavender Beds", "Rain")]
        [TestCase(2018, 12, 13, 17, 06, 40, "The Lavender Beds", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "The Lavender Beds", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "The Lavender Beds", "Clear Skies")]
        [TestCase(2018, 12, 13, 14, 23, 20, "Azys Lla", "Fair Skies")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Azys Lla", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Azys Lla", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Azys Lla", "Fair Skies")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Azys Lla", "Thunder")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Azys Lla", "Clouds")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Azys Lla", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Azys Lla", "Thunder")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Azys Lla", "Clouds")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Azys Lla", "Clouds")]
        [TestCase(2018, 12, 13, 14, 23, 20, "Idyllshire", "Clouds")]
        [TestCase(2018, 12, 13, 14, 46, 40, "Idyllshire", "Rain")]
        [TestCase(2018, 12, 13, 15, 10, 0, "Idyllshire", "Rain")]
        [TestCase(2018, 12, 13, 15, 33, 20, "Idyllshire", "Clouds")]
        [TestCase(2018, 12, 13, 15, 56, 40, "Idyllshire", "Fair Skies")]
        [TestCase(2018, 12, 13, 16, 20, 0, "Idyllshire", "Clear Skies")]
        [TestCase(2018, 12, 13, 16, 43, 20, "Idyllshire", "Clouds")]
        [TestCase(2018, 12, 13, 17, 06, 40, "Idyllshire", "Fair Skies")]
        [TestCase(2018, 12, 13, 17, 30, 0, "Idyllshire", "Clear Skies")]
        [TestCase(2018, 12, 13, 17, 53, 20, "Idyllshire", "Clear Skies")]
        public void GetWeatherNameForTimeReturnsCorrectResultForZone(
            int year, int month, int day, int hour, int minute,
            int second, string zone, string expectedWeather)
        {
            var d = new DateTime(year, month, day, hour, minute, second);
            var shifted = WeatherService.GetWeatherTimeFloor(d);
            var weather = WeatherService.GetWeatherNameForTime(shifted, zone);
            
            weather.Should().Be(expectedWeather);
        }

        #endregion
    }
}
