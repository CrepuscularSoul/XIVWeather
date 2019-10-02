using System;

namespace WeatherApp.Domain.Models.Locations
{
    /// <summary>
    /// Class defining a weather breakpoint and associated weather.
    /// </summary>
    public class WeatherBreakpoint
    {
        /// <summary>
        /// The value for the breakpoint.
        /// </summary>
        /// <remarks>
        /// When a weather chance is less than this value, it is potentially applicable unless there
        /// is another breakpoint with a lower value that is also higher than the weather chance.
        /// </remarks>
        public ulong Value { get; set; }

        /// <summary>
        /// The Weather that will occur if this breakpoint is satisfied.
        /// </summary>
        public Enums.Weather Weather { get; set; }
    }
}