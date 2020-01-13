using System.Collections.Generic;
using System.Linq;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Gathering;

namespace WeatherApp.Domain.Repository
{
    /// <summary>
    /// The gathering repository.
    /// </summary>
    public static class GatheringRepository
    {
        /// <summary>
        /// The gathering items.
        /// </summary>
        public static readonly List<GatheringItem> GatheringItems;

        static GatheringRepository()
        {
            GatheringItems = FillGatheringItems();
        }

        /// <summary>
        /// Helper method to fill the gathering items collection.
        /// </summary>
        private static List<GatheringItem> FillGatheringItems()
        {
            return GetUnspoiledItems()
                .Concat(GetLegendaryItems())
                .Concat(GetEphemeralItems())
                .ToList();
        }

        /// <summary>
        /// Gets the list of items found at unspoiled nodes.
        /// </summary>
        private static List<GatheringItem> GetUnspoiledItems()
        {
            return new List<GatheringItem>
            {
                Build("White Oak Branch", Constants.Zones.Kholusia, 11, 29, 10, 12, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.Yellow),
                Build("Broad Beans", Constants.Zones.IlMheg, 25, 36, 0, 2, true, Enums.NodeType.Unspoiled ,Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.Yellow),
                Build("Peppermint", Constants.Zones.Lakeland, 27, 21, 10, 12, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.None),
                Build("Mist Spinach", Constants.Zones.RaktikaGreatwood, 35, 22, 0, 2, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.White),
                Build("Lemonette", Constants.Zones.Kholusia, 20, 27, 6, 8, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.White),
                Build("Russet Popoto", Constants.Zones.AmhAraeng, 20, 17, 8, 10, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.Yellow),

                Build("Raw Hematite", Constants.Zones.Kholusia, 33, 23, 2, 4, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.Yellow),
                Build("Raw Lazurite", Constants.Zones.RaktikaGreatwood, 19, 20, 4, 6, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.None),
                Build("Raw Diaspore", Constants.Zones.IlMheg, 26, 13, 6, 8, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.Yellow),
                Build("Raw Triplite", Constants.Zones.AmhAraeng, 19, 28, 0, 2, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.Yellow),
                Build("Raw Onyx", Constants.Zones.Tempest, 16, 21, 0, 2, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.White),
                Build("Raw Petalite", Constants.Zones.Lakeland, 28, 33, 6, 8, true, Enums.NodeType.Unspoiled, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.White),
            };
        }

        /// <summary>
        /// Gets the list of items found at legendary nodes.
        /// </summary>
        private static List<GatheringItem> GetLegendaryItems()
        {
            return new List<GatheringItem>
            {
                Build("Sandalwood Log", Constants.Zones.RaktikaGreatwood, 25, 37, 2, 4, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.White),
                Build("Sandalwood Sap", Constants.Zones.RaktikaGreatwood, 25, 37, 2, 4, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.None),
                Build("Ethereal Cocoon", Constants.Zones.Lakeland, 26, 10, 8, 10, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Botanist, Enums.ScripType.White),

                Build("Prismstone", Constants.Zones.IlMheg, 30, 21, 4, 6, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.White),
                Build("Berrylium Ore", Constants.Zones.IlMheg, 30, 21, 4, 6, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.None),
                Build("Tungsten Ore", Constants.Zones.Tempest, 32, 7, 10, 12, true, Enums.NodeType.Legendary, Enums.Aethersand.None, Enums.ClusterType.None, Enums.Gatherer.Miner, Enums.ScripType.White),
            };
        }

        /// <summary>
        /// Gets the list of items found at ephemeral nodes.
        /// </summary>
        private static List<GatheringItem> GetEphemeralItems()
        {
            return new List<GatheringItem>
            {
                Build("Shade Quartz", Constants.Zones.Kholusia, 22, 18, 16, 20, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Agedeep, Enums.ClusterType.Fire, Enums.Gatherer.Miner, Enums.ScripType.None),
                Build("Solarite", Constants.Zones.Lakeland, 29, 24, 8, 12, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Scuroglow, Enums.ClusterType.Lightning, Enums.Gatherer.Miner, Enums.ScripType.None),
                Build("Gale Rock", Constants.Zones.RaktikaGreatwood, 25, 28, 0, 4, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Chiaroglow, Enums.ClusterType.Water, Enums.Gatherer.Miner, Enums.ScripType.None),
                Build("Bog Sage", Constants.Zones.Lakeland, 25, 29, 12, 16, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Agewood, Enums.ClusterType.Ice, Enums.Gatherer.Botanist, Enums.ScripType.None),
                Build("White Clay", Constants.Zones.Kholusia, 13, 13, 20, 0, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Chiaroglow, Enums.ClusterType.Earth, Enums.Gatherer.Botanist, Enums.ScripType.None),
                Build("Sweet Marjoram", Constants.Zones.RaktikaGreatwood, 29, 24, 4, 8, false, Enums.NodeType.Ephemeral, Enums.Aethersand.Scuroglow, Enums.ClusterType.Wind, Enums.Gatherer.Botanist, Enums.ScripType.None),
            };
        }

        /// <summary>
        /// Builds a gathering item with the specified details.
        /// </summary>
        private static GatheringItem Build(
            string name, string zone, float xAxis, float yAxis, uint? start, uint? end,
            bool amAndPm, Enums.NodeType nodeType, Enums.Aethersand sandType, 
            Enums.ClusterType clusterType, Enums.Gatherer gatherer, Enums.ScripType scripType) 
            => new GatheringItem
            {
                Name = name,
                Zone = zone,
                Location = new Location(xAxis, yAxis),
                NodeAvailability = start == null && end == null
                                   ? new NodeAvailability() 
                                   : new NodeAvailability(start.Value, end.Value, amAndPm),
                NodeType = nodeType,
                ReducesTo = sandType,
                ClusterType = clusterType,
                Gatherer = gatherer,
                ScripType = scripType
            };
    }
}