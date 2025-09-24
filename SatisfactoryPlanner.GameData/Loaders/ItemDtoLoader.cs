using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads item DTOs from JSON
/// </summary>
public class ItemDtoLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private List<ItemDto>? _cachedItems;

    public ItemDtoLoader(string filePath)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
    }

    /// <summary>
    /// Loads item DTOs from the game data file
    /// </summary>
    public async Task<List<ItemDto>> LoadItemDtosAsync()
    {
        if (_cachedItems != null)
            return _cachedItems;

        var gameData = await _jsonLoader.LoadAsync();
        _cachedItems = gameData.Items;
        return _cachedItems;
    }

    /// <summary>
    /// Clears the cached data
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedItems = null;
    }
}