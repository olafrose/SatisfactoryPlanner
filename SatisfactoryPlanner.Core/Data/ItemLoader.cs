using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Loads items from JSON and converts them to domain models
/// </summary>
public class ItemLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private List<Item>? _cachedItems;

    public ItemLoader(string filePath)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
    }

    /// <summary>
    /// Loads items and converts them to domain models
    /// </summary>
    public async Task<List<Item>> LoadItemsAsync()
    {
        if (_cachedItems != null)
            return _cachedItems;

        var gameData = await _jsonLoader.LoadAsync();
        _cachedItems = gameData.Items.Select(ConvertToItem).ToList();
        return _cachedItems;
    }

    /// <summary>
    /// Loads items and returns them as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Item>> LoadItemsLookupAsync()
    {
        var items = await LoadItemsAsync();
        return items.ToDictionary(i => i.Id, i => i);
    }

    private static Item ConvertToItem(ItemDto dto)
    {
        return new Item
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Category = Enum.TryParse<ItemCategory>(dto.Category, out var category) ? category : ItemCategory.Other,
            IsRawResource = dto.IsRawResource,
            IconPath = dto.IconPath
        };
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