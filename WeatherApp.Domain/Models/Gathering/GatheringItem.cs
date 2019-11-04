using WeatherApp.Domain.Models.Locations;
using WeatherApp.Domain.Services;

namespace WeatherApp.Domain.Models.Gathering
{
    /// <summary>
    /// Information relating to an item that can be gathered
    /// </summary>
    public class GatheringItem
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The zone the item can be found in.
        /// </summary>
        public string Zone { get; set; }

        /// <summary>
        /// The (x, y) location of the node.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Details about when the node is available.
        /// </summary>
        public NodeAvailability NodeAvailability { get; set; }

        /// <summary>
        /// The type of the node.
        /// </summary>
        public Enums.NodeType NodeType { get; set; }

        /// <summary>
        /// For Ephemeral Nodes, the type of sand that can be acquired from Aetherial Reduction.
        /// </summary>
        public Enums.Aethersand ReducesTo { get; set; }

        /// <summary>
        /// The type of shards, crystals, or clusters that can acquired from the node or
        /// via Aetherial Reduction.
        /// </summary>
        public Enums.ClusterType ClusterType { get; set; }

        /// <summary>
        /// The type of gatherer that can collect this item.
        /// </summary>
        public Enums.Gatherer Gatherer { get; set; }

        /// <summary>
        /// The type of scrips that can be obtained from turning in collectables of this item.
        /// </summary>
        public Enums.ScripType ScripType { get; set; }

        /// <summary>
        /// Gets the property names in order they should be displayed.
        /// </summary>
        /// <returns></returns>
        public static string[] GetRelevantPropertyNames() 
            => new[] 
            { 
                "Item",
                "Start", 
                "Am/Pm",
                "Zone", 
                "Location",  
                "Type", 
                "Scrip", 
                "Sand", 
                "Cluster" };

        /// <summary>
        /// Returns relevant details about the item in a string array.
        /// </summary>
        public string[] ToStringArray()
        {
            return new[]
            {
                Name,
                NodeAvailability.GetStartTime(),
                NodeAvailability.GetAmPm(),
                Zone,
                Location.ToString(),
                NodeType.ToString(),
                ScripType.ToString(),
                ReducesTo.ToString(),
                ClusterType.ToString()
            };
        }
    }
}