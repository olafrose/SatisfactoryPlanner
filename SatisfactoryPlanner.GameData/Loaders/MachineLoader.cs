using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads machines from JSON and converts them to domain models (obsolete - use BuildingLoader)
/// </summary>
[Obsolete("Use BuildingLoader instead of MachineLoader to align with wiki terminology")]
public class MachineLoader
{
    private readonly JsonDataLoader<List<MachineDto>> _jsonLoader;
    private List<Machine>? _cachedMachines;

    public MachineLoader(string gameDataDirectory)
    {
        var machinesFilePath = Path.Combine(gameDataDirectory, "machines.json");
        _jsonLoader = new JsonDataLoader<List<MachineDto>>(machinesFilePath);
    }

    /// <summary>
    /// Loads machines and converts them to domain models (obsolete - use BuildingLoader)
    /// </summary>
    [Obsolete("Use BuildingLoader.LoadBuildingsAsync instead to align with wiki terminology")]
    public async Task<List<Machine>> LoadMachinesAsync()
    {
        if (_cachedMachines != null)
            return _cachedMachines;

        var machineDtos = await _jsonLoader.LoadAsync();
        _cachedMachines = machineDtos.Select(ConvertToMachine).ToList();
        return _cachedMachines;
    }

    /// <summary>
    /// Loads machines as a lookup dictionary (obsolete - use BuildingLoader)
    /// </summary>
    [Obsolete("Use BuildingLoader.LoadBuildingsLookupAsync instead to align with wiki terminology")]
    public async Task<Dictionary<string, Machine>> LoadMachinesLookupAsync()
    {
        var machines = await LoadMachinesAsync();
        return machines.ToDictionary(m => m.Id, m => m);
    }

    /// <summary>
    /// Converts machine DTO to domain model
    /// </summary>
    [Obsolete("Internal method - will be removed when MachineLoader is fully deprecated")]
    private static Machine ConvertToMachine(MachineDto dto)
    {
        return new Machine
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Type = Enum.TryParse<MachineType>(dto.Type, out var type) ? type : MachineType.Constructor,
            ProductionSpeed = dto.ProductionSpeed,
            PowerConsumption = dto.PowerConsumption,
            MaxInputConnections = dto.MaxInputConnections,
            MaxOutputConnections = dto.MaxOutputConnections,
            CanOverclock = dto.CanOverclock
        };
    }

    /// <summary>
    /// Clears cached machines
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedMachines = null;
    }
}