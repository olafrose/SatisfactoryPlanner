using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads items from JSON and converts them to domain models
/// </summary>
public class ItemLoader
{
    private readonly JsonDataLoader<List<ItemDto>> _jsonLoader;
    private List<Item>? _cachedItems;

    public ItemLoader(string gameDataDirectory)
    {
        var itemsFilePath = Path.Combine(gameDataDirectory, "items.json");
        _jsonLoader = new JsonDataLoader<List<ItemDto>>(itemsFilePath);
    }

    /// <summary>
    /// Loads items and converts them to domain models
    /// </summary>
    public async Task<List<Item>> LoadItemsAsync()
    {
        if (_cachedItems != null)
            return _cachedItems;

        var itemDtos = await _jsonLoader.LoadAsync();
        _cachedItems = itemDtos.Select(ConvertToItem).ToList();
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
        var item = new Item
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            IsRawResource = dto.IsRawResource,
            IconPath = dto.IconPath
        };

        // Parse primary category
        if (Enum.TryParse<ItemCategory>(dto.Category, out var primaryCategory))
        {
            item.PrimaryCategory = primaryCategory;
            item.AddCategory(primaryCategory); // Also add to Categories collection
        }
        else
        {
            item.PrimaryCategory = ItemCategory.Other;
            item.AddCategory(ItemCategory.Other);
        }

        // TODO: In the future, parse multiple categories from JSON if available
        // For now, we only have single category data, so we use the primary category

        return item;
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