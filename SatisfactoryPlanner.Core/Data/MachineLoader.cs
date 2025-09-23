using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Loads machines from JSON and converts them to domain models
/// </summary>
public class MachineLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private List<Machine>? _cachedMachines;

    public MachineLoader(string filePath)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
    }

    /// <summary>
    /// Loads machines and converts them to domain models
    /// </summary>
    public async Task<List<Machine>> LoadMachinesAsync()
    {
        if (_cachedMachines != null)
            return _cachedMachines;

        var gameData = await _jsonLoader.LoadAsync();
        _cachedMachines = gameData.Machines.Select(ConvertToMachine).ToList();
        return _cachedMachines;
    }

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
    /// Clears the cached data
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedMachines = null;
    }
}