using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of building repository that loads from JSON data
/// </summary>
public class InMemoryBuildingRepository : IBuildingRepository
{
    private readonly GameDataService _gameDataService;
    private List<Building>? _buildings;

    public InMemoryBuildingRepository(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
    }

    private async Task EnsureBuildingsLoadedAsync()
    {
        if (_buildings == null)
        {
            // Load buildings using the new LoadBuildingsAsync method
            _buildings = await _gameDataService.LoadBuildingsAsync();
        }
    }

    public async Task<List<Building>> GetAllBuildingsAsync()
    {
        await EnsureBuildingsLoadedAsync();
        return _buildings!;
    }

    public async Task<Building?> GetBuildingByIdAsync(string id)
    {
        await EnsureBuildingsLoadedAsync();
        return _buildings!.FirstOrDefault(b => b.Id == id);
    }

    public async Task<List<Building>> GetBuildingsForRecipeAsync(string recipeId)
    {
        await EnsureBuildingsLoadedAsync();
        
        return _buildings!.Where(b => 
        {
            return 
            (recipeId.Contains("ingot") && b.Type == BuildingType.Smelter) ||
            (recipeId.Contains("plate") || recipeId.Contains("rod") || recipeId.Contains("screw") ||
             recipeId.Contains("wire") || recipeId.Contains("cable") || recipeId.Contains("concrete") ||
             recipeId.Contains("biomass") || recipeId.Contains("solid_biofuel") ||
             recipeId.Contains("cast") || recipeId.Contains("fused")) && b.Type == BuildingType.Constructor ||
            (recipeId.Contains("rotor") || recipeId.Contains("modular_frame") || 
             recipeId.Contains("reinforced_iron_plate") || recipeId.Contains("copper_sheet") ||
             recipeId.Contains("smart_plating") || recipeId.Contains("adhered")) && b.Type == BuildingType.Assembler;
        }).ToList();
    }

    public async Task<List<Building>> GetBuildingsByTypeAsync(BuildingType type)
    {
        await EnsureBuildingsLoadedAsync();
        return _buildings!.Where(b => b.Type == type).ToList();
    }

    public async Task<List<Building>> GetBuildingsByTierAsync(int maxTier)
    {
        await EnsureBuildingsLoadedAsync();
        // Since buildings are now unlocked by milestones, return all buildings
        // Tier-based filtering should be handled at a higher level using milestone progression
        return _buildings!.ToList();
    }
}