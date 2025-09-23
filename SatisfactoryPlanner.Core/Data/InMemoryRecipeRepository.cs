using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of recipe repository that loads from JSON data
/// </summary>
public class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly GameDataLoader _dataLoader;
    private List<Recipe>? _recipes;

    public InMemoryRecipeRepository(GameDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public InMemoryRecipeRepository(string dataFilePath)
    {
        _dataLoader = new GameDataLoader(dataFilePath);
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_recipes == null)
        {
            var (_, recipes, _, _) = await _dataLoader.LoadModelsAsync();
            _recipes = recipes;
        }
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        await EnsureDataLoadedAsync();
        return _recipes!;
    }

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        await EnsureDataLoadedAsync();
        return _recipes!.FirstOrDefault(r => r.Id == id);
    }

    public async Task<List<Recipe>> GetRecipesForOutputAsync(string itemId)
    {
        await EnsureDataLoadedAsync();
        return _recipes!.Where(r => r.Outputs.Any(o => o.Item.Id == itemId)).ToList();
    }

    public async Task<List<Recipe>> GetRecipesForInputAsync(string itemId)
    {
        await EnsureDataLoadedAsync();
        return _recipes!.Where(r => r.Inputs.Any(i => i.Item.Id == itemId)).ToList();
    }

    public async Task<List<Recipe>> GetAlternateRecipesAsync()
    {
        await EnsureDataLoadedAsync();
        return _recipes!.Where(r => r.IsAlternate).ToList();
    }

    public async Task<List<Recipe>> GetRecipesByTierAsync(int maxTier)
    {
        await EnsureDataLoadedAsync();
        // Since recipes are now unlocked by milestones, return all recipes
        // Milestone filtering should happen at a higher level
        return _recipes!.ToList();
    }
}