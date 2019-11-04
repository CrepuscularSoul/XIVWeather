using System;

namespace WeatherApp.Domain.Models.Gathering
{
    /// <summary>
    /// Class describing the availability of a given node.
    /// </summary>
    public class NodeAvailability
    {
        /// <summary>
        /// Gets a value indicating whether this node is always available.
        /// </summary>
        public bool IsAlwaysAvailable => StartTime == null;

        /// <summary>
        /// Gets a value indicating whether this node is available both am and pm.
        /// </summary>
        public bool IsAvailableAmAndPm { get; }

        /// <summary>
        /// The time the node starts. If the node is available both am/pm this will be the am hour.
        /// </summary>
        public uint? StartTime { get; }

        /// <summary>
        /// The time the node expires.
        /// </summary>
        public uint? EndTime { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NodeAvailability() {}

        /// <summary>
        /// Constructs an instance of the class with the supplied details.
        /// </summary>
        public NodeAvailability(uint start, uint end, bool amAndPm)
        {
            if (start > 23)
                throw new ArgumentOutOfRangeException(nameof(start), "Start Time must be between 0 (midnight) and 23.");
            if (end > 23)
                throw new ArgumentOutOfRangeException(nameof(start), "End Time must be between 0 (midnight) and 23.");
            if (start == end)
                throw new InvalidOperationException("Start and End time cannot be the same");

            StartTime = start;
            EndTime = end;
            IsAvailableAmAndPm = amAndPm;
        }

        /// <summary>
        /// Helper method to get a formatted start time for the UI.
        /// </summary>
        public string GetStartTime()
        {
            return StartTime switch
            {
                _ when IsAlwaysAvailable => "Always",
                _ when StartTime == 0 || StartTime == 12 => "12",
                _ when StartTime < 10 => $"0{StartTime}",
                _ when StartTime >= 10 && StartTime <= 12 => StartTime.ToString(),
                _ when StartTime >= 22 => $"{StartTime - 12}",
                _ => $"0{StartTime - 12}"
            };
        }

        /// <summary>
        /// Helper method to get a formatted value for am/pm availability.
        /// </summary>
        public string GetAmPm()
        {
            return StartTime switch
            {
                _ when IsAlwaysAvailable => "Always",
                _ when IsAvailableAmAndPm => "Both",
                _ when StartTime < 12 => "am",
                _ => "pm"
            };
        }
    }
}