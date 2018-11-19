using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

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
                    DesiredTimes = new List<string> {"0"},
                    MaxMatches = 5,
                    MaxTries = 2000
                });

            results.Should().NotBeNull();
            results.Count.Should().BeGreaterThan(0);
            results.Count.Should().Be(5);
        }

        [Test]
        public void GetWeatherNameForTimeReturnsCorrectResult()
        {
            var dateTime = new DateTime(2018, 10, 19, 13, 27, 0);
            
            var shiftedTime = WeatherService.GetWeatherTimeFloor(dateTime);
            var weather = WeatherService.GetWeatherNameForTime(shiftedTime, "The Azim Steppe");
            var eorzeaHour = WeatherService.GetEorzeaHour(shiftedTime);
            
            eorzeaHour.Should().Be(0);
            weather.Should().Be("Wind");
        }

        [Test]
        public void asdf()
        {
            var dateTime = new DateTime(2018, 10, 19, 13, 27, 0);
            
            var shiftedTime = WeatherService.GetWeatherTimeFloor(dateTime);
            var weatherChance = WeatherService.CalculateForecastTarget(shiftedTime);

            weatherChance.Should().BeLessThan(10);
        }
    }
}
