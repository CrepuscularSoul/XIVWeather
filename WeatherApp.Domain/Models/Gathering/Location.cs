namespace WeatherApp.Domain.Models.Gathering
{
    /// <summary>
    /// Describes a location in Eorzea.
    /// </summary>
    public sealed class Location
    {
        /// <summary>
        /// The x axis on the map.
        /// </summary>
        public float XAxis { get; }

        /// <summary>
        /// The y axis on the map.
        /// </summary>
        public float YAxis { get; }

        /// <summary>
        /// Constructs a new instance of the location.
        /// </summary>
        public Location(float x, float y)
        {
            XAxis = x;
            YAxis = y;
        }

        /// <summary>
        /// Converts the location to a UI friendly string.
        /// </summary>
        public override string ToString() => $"X: {XAxis}, Y: {YAxis}";
    }
}