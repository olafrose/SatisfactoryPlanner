using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of item repository that loads from JSON data
/// </summary>
public class InMemoryItemRepository : IItemRepository
{
    private readonly GameDataLoader _dataLoader;
    private List<Item>? _items;

    public InMemoryItemRepository(GameDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public InMemoryItemRepository(string dataFilePath)
    {
        _dataLoader = new GameDataLoader(dataFilePath);
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_items == null)
        {
            var (items, _, _, _) = await _dataLoader.LoadModelsAsync();
            _items = items;
        }
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        await EnsureDataLoadedAsync();
        return _items!;
    }

    public async Task<Item?> GetItemByIdAsync(string id)
    {
        await EnsureDataLoadedAsync();
        return _items!.FirstOrDefault(i => i.Id == id);
    }

    public async Task<List<Item>> GetItemsByCategoryAsync(ItemCategory category)
    {
        await EnsureDataLoadedAsync();
        return _items!.Where(i => i.Category == category).ToList();
    }

    public async Task<List<Item>> GetRawResourcesAsync()
    {
        await EnsureDataLoadedAsync();
        return _items!.Where(i => i.IsRawResource).ToList();
    }
}