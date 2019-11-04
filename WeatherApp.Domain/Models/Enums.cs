
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace WeatherApp.Domain.Models
{
    /// <summary>
    /// Class containing enums used throughout the application
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Initializes the WeatherMapping dictionary via reflection. This is primarily
        /// so objects can work with the weather enum, but easily translate that to something
        /// useful for displaying in the app by pulling them from the constants class.
        /// </summary>
        static Enums()
        {
            WeatherMapping = new Dictionary<Weather, string>();

            var enumValues = Enum.GetValues(typeof(Weather));
            foreach (var value in enumValues)
            {
                var constant = typeof(Constants.Weather).GetFields()
                    .FirstOrDefault(x => x.Name == Enum.GetName(typeof(Weather), value));
                var constantValue = constant?.GetValue(constant);
                WeatherMapping.Add((Weather)value, constantValue?.ToString());
            }
        }

        /// <summary>
        /// Weather conditions available in the game.
        /// </summary>
        public enum Weather
        {
            Blizzards,
            ClearSkies,
            Clouds,
            DustStorms,
            FairSkies,
            Fog,
            Gales,
            Gloom,
            HeatWaves,
            Rain,
            Showers,
            Snow,
            Thunder,
            Thunderstorms,
            UmbralStatic,
            UmbralWind,
            Wind,
        }

        /// <summary>
        /// The types of shards, crystals, and clusters.
        /// </summary>
        public enum ClusterType
        {
            None,
            Lightning,
            Fire,
            Ice,
            Water,
            Earth,
            Wind
        }

        /// <summary>
        /// The types of gatherers.
        /// </summary>
        public enum Gatherer
        {
            Botanist,
            Miner,
            Fisher
        }

        /// <summary>
        /// The type of a gathering node.
        /// </summary>
        public enum NodeType
        {
            Standard,
            Unspoiled,
            Legendary,
            Ephemeral
        }

        /// <summary>
        /// The types of aethersand.
        /// </summary>
        public enum Aethersand
        {
            None,
            Scuroglow,
            Chiarglow,
            Agedeep,
            Agewood
        }

        /// <summary>
        /// The types of scrips.
        /// </summary>
        public enum ScripType
        {
            None,
            Yellow, 
            White
        }

        /// <summary>
        /// Maps an enum value to a display friendly string.
        /// </summary>
        public static Dictionary<Weather, string> WeatherMapping { get; }
    }
}