using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of machine repository that loads from JSON data (deprecated - use InMemoryBuildingRepository)
/// </summary>
[Obsolete("Use InMemoryBuildingRepository instead of InMemoryMachineRepository to align with wiki terminology")]
public class InMemoryMachineRepository : InMemoryBuildingRepository, IMachineRepository
{
    public InMemoryMachineRepository(GameDataService gameDataService) : base(gameDataService)
    {
    }

    public async Task<List<Machine>> GetAllMachinesAsync()
    {
        var buildings = await GetAllBuildingsAsync();
        return buildings.Cast<Machine>().ToList();
    }

    public async Task<Machine?> GetMachineByIdAsync(string id)
    {
        var building = await GetBuildingByIdAsync(id);
        return building as Machine;
    }

    public async Task<List<Machine>> GetMachinesForRecipeAsync(string recipeId)
    {
        var buildings = await GetBuildingsForRecipeAsync(recipeId);
        return buildings.Cast<Machine>().ToList();
    }

    public async Task<List<Machine>> GetMachinesByTypeAsync(MachineType type)
    {
        var buildings = await GetBuildingsByTypeAsync((BuildingType)type);
        return buildings.Cast<Machine>().ToList();
    }

    public async Task<List<Machine>> GetMachinesByTierAsync(int maxTier)
    {
        var buildings = await GetBuildingsByTierAsync(maxTier);
        return buildings.Cast<Machine>().ToList();
    }
}