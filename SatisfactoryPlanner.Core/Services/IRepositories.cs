using SatisfactoryPlanner.Core.Models;

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
/// Repository interface for managing machines
/// </summary>
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