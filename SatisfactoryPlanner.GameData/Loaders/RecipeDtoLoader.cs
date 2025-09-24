using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads recipe DTOs from JSON
/// </summary>
public class RecipeDtoLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private List<RecipeDto>? _cachedRecipes;

    public RecipeDtoLoader(string filePath)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
    }

    /// <summary>
    /// Loads recipe DTOs from the game data file
    /// </summary>
    public async Task<List<RecipeDto>> LoadRecipeDtosAsync()
    {
        if (_cachedRecipes != null)
            return _cachedRecipes;

        var gameData = await _jsonLoader.LoadAsync();
        _cachedRecipes = gameData.Recipes;
        return _cachedRecipes;
    }

    /// <summary>
    /// Clears the cached data
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedRecipes = null;
    }
}