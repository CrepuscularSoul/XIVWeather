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
