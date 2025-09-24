using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of item repository that loads from JSON data
/// </summary>
public class InMemoryItemRepository : IItemRepository
{
    private readonly GameDataService _gameDataService;
    private List<Item>? _items;

    public InMemoryItemRepository(GameDataService gameDataService)
    {
        _gameDataService = gameDataService;
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_items == null)
        {
            _items = await _gameDataService.LoadItemsAsync();
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