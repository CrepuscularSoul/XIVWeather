using System.Collections.Generic;
using System.Linq;
using WeatherApp.Domain.Models;
using WeatherApp.Domain.Models.Gathering;
using WeatherApp.Domain.Repository;

namespace WeatherApp.Domain.Services
{
    /// <summary>
    /// Gathering services.
    /// </summary>
    public static class GatheringService
    {
        /// <summary>
        /// Gets all gathering items.
        /// </summary>
        public static List<GatheringItem> GetAllGatheringItems()
            => GatheringRepository.GatheringItems;

        /// <summary>
        /// Gets details about a specific item by name.
        /// </summary>
        public static GatheringItem GetItemDetails(string itemName)
            => GatheringRepository.GatheringItems.FirstOrDefault(x => x.Name == itemName);

        /// <summary>
        /// Searches for items whose name contains the search text.
        /// </summary>
        public static List<GatheringItem> Search(string partialName)
            => GatheringRepository.GatheringItems.Where(x => x.Name.Contains(partialName)).OrderBy(x => x.Name).ToList();

        /// <summary>
        /// Gets a scrip rotation based on the type of scrip selected.
        /// </summary>
        public static List<GatheringItem> GetScripRotation(Enums.ScripType scripType) 
            => GatheringRepository.GatheringItems
                .Where(x => x.ScripType == scripType)
                .OrderBy(x => x.NodeAvailability.StartTime)
                .ToList();

        /// <summary>
        /// Gets the ephemeral node rotation.
        /// </summary>
        public static List<GatheringItem> GetEphemeralNodeRotation()
            => GatheringRepository.GatheringItems
                .Where(x => x.NodeType == Enums.NodeType.Ephemeral)
                .OrderBy(x => x.NodeAvailability.StartTime)
                .ToList();

        /// <summary>
        /// Gets items of the specified type. Sorts by name or start time based on the supplied flag.
        /// </summary>
        public static List<GatheringItem> GetItemsOfType(Enums.NodeType type, bool sortByStartTime)
        {
            var subset = GatheringRepository.GatheringItems
                .Where(x => x.NodeType == type);

            return sortByStartTime
                ? subset.OrderBy(x => x.NodeAvailability.StartTime).ToList()
                : subset.OrderBy(x => x.Name).ToList();
        }
    }
}