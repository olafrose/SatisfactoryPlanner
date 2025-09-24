using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders
{
    /// <summary>
    /// Loads building data from JSON files
    /// </summary>
    public class BuildingLoader
    {
        private readonly string _dataDirectory;
        private List<Building>? _cachedBuildings;

        public BuildingLoader(string dataDirectory)
        {
            _dataDirectory = dataDirectory ?? throw new ArgumentNullException(nameof(dataDirectory));
        }

        /// <summary>
        /// Loads all buildings from the game data
        /// </summary>
        public async Task<List<Building>> LoadBuildingsAsync()
        {
            if (_cachedBuildings != null)
                return _cachedBuildings;

            var machineLoader = new MachineLoader(_dataDirectory);
            var machines = await machineLoader.LoadMachinesAsync();
            
            // Convert machines to buildings
            _cachedBuildings = machines.Cast<Building>().ToList();
            
            return _cachedBuildings;
        }

        /// <summary>
        /// Loads buildings as a lookup dictionary by ID
        /// </summary>
        public async Task<Dictionary<string, Building>> LoadBuildingsLookupAsync()
        {
            var buildings = await LoadBuildingsAsync();
            return buildings.ToDictionary(b => b.Id, b => b);
        }
    }
}