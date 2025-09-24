using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Core.Services;

/// <summary>
/// Repository interface for managing recipes
/// </summary>
public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllRecipesAsync();
    Task<Recipe?> GetRecipeByIdAsync(string id);
    Task<List<Recipe>> GetRecipesForOutputAsync(string itemId);
    Task<List<Recipe>> GetRecipesForInputAsync(string itemId);
    Task<List<Recipe>> GetRecipesByTierAsync(int maxTier);
    Task<List<Recipe>> GetAlternateRecipesAsync();
}

/// <summary>
/// Repository interface for managing buildings
/// </summary>
public interface IBuildingRepository
{
    Task<List<Building>> GetAllBuildingsAsync();
    Task<Building?> GetBuildingByIdAsync(string id);
    Task<List<Building>> GetBuildingsForRecipeAsync(string recipeId);
    Task<List<Building>> GetBuildingsByTypeAsync(BuildingType type);
    Task<List<Building>> GetBuildingsByTierAsync(int maxTier);
}

/// <summary>
/// Repository interface for managing machines (deprecated - use IBuildingRepository)
/// </summary>
[Obsolete("Use IBuildingRepository instead of IMachineRepository to align with wiki terminology")]
public interface IMachineRepository
{
    Task<List<Machine>> GetAllMachinesAsync();
    Task<Machine?> GetMachineByIdAsync(string id);
    Task<List<Machine>> GetMachinesForRecipeAsync(string recipeId);
    Task<List<Machine>> GetMachinesByTypeAsync(MachineType type);
    Task<List<Machine>> GetMachinesByTierAsync(int maxTier);
}

/// <summary>
/// Repository interface for managing items
/// </summary>
public interface IItemRepository
{
    Task<List<Item>> GetAllItemsAsync();
    Task<Item?> GetItemByIdAsync(string id);
    Task<List<Item>> GetItemsByCategoryAsync(ItemCategory category);
    Task<List<Item>> GetRawResourcesAsync();
}

/// <summary>
/// Repository interface for managing milestones
/// </summary>
public interface IMilestoneRepository
{
    Task<List<Milestone>> GetAllMilestonesAsync();
    Task<Milestone?> GetMilestoneByIdAsync(string id);
    Task<List<Milestone>> GetMilestonesByTierAsync(int tier);
    Task<List<string>> GetRecipeIdsUnlockedByMilestoneAsync(string milestoneId);
    Task<List<string>> GetMachineIdsUnlockedByMilestoneAsync(string milestoneId);
    Task<bool> AreMilestonePrerequisitesMetAsync(string milestoneId, IEnumerable<string> completedMilestones);
}