using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// File-based implementation of item repository that loads from JSON
/// </summary>
public class FileBasedItemRepository : IItemRepository
{
    private readonly GameDataLoader _dataLoader;
    private List<Item>? _items;

    public FileBasedItemRepository(GameDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public FileBasedItemRepository(string dataFilePath)
    {
        _dataLoader = new GameDataLoader(dataFilePath);
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

    private async Task EnsureDataLoadedAsync()
    {
        if (_items == null)
        {
            var (items, _) = await _dataLoader.LoadModelsAsync();
            _items = items;
        }
    }
}

/// <summary>
/// Legacy in-memory implementation of item repository with hardcoded Satisfactory game data
/// </summary>
public class InMemoryItemRepository : IItemRepository
{
    private readonly List<Item> _items;

    public InMemoryItemRepository()
    {
        _items = InitializeItems();
    }

    public Task<List<Item>> GetAllItemsAsync()
    {
        return Task.FromResult(_items);
    }

    public Task<Item?> GetItemByIdAsync(string id)
    {
        return Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
    }

    public Task<List<Item>> GetItemsByCategoryAsync(ItemCategory category)
    {
        return Task.FromResult(_items.Where(i => i.Category == category).ToList());
    }

    public Task<List<Item>> GetRawResourcesAsync()
    {
        return Task.FromResult(_items.Where(i => i.IsRawResource).ToList());
    }

    private List<Item> InitializeItems()
    {
        return new List<Item>
        {
            // Raw Resources
            new Item
            {
                Id = "iron_ore",
                Name = "Iron Ore",
                Description = "Used for smelting into Iron Ingots",
                Category = ItemCategory.RawResource,
                IsRawResource = true
            },
            new Item
            {
                Id = "copper_ore",
                Name = "Copper Ore",
                Description = "Used for smelting into Copper Ingots",
                Category = ItemCategory.RawResource,
                IsRawResource = true
            },
            new Item
            {
                Id = "limestone",
                Name = "Limestone",
                Description = "Used for creating Concrete",
                Category = ItemCategory.RawResource,
                IsRawResource = true
            },
            new Item
            {
                Id = "coal",
                Name = "Coal",
                Description = "Used for power generation and steel production",
                Category = ItemCategory.RawResource,
                IsRawResource = true
            },
            
            // Ingots
            new Item
            {
                Id = "iron_ingot",
                Name = "Iron Ingot",
                Description = "Refined iron for crafting",
                Category = ItemCategory.Ingot
            },
            new Item
            {
                Id = "copper_ingot",
                Name = "Copper Ingot",
                Description = "Refined copper for crafting",
                Category = ItemCategory.Ingot
            },
            
            // Basic Parts
            new Item
            {
                Id = "iron_plate",
                Name = "Iron Plate",
                Description = "Basic iron construction component",
                Category = ItemCategory.BasicPart
            },
            new Item
            {
                Id = "iron_rod",
                Name = "Iron Rod",
                Description = "Basic iron construction component",
                Category = ItemCategory.BasicPart
            },
            new Item
            {
                Id = "screw",
                Name = "Screw",
                Description = "Basic fastening component",
                Category = ItemCategory.BasicPart
            },
            new Item
            {
                Id = "wire",
                Name = "Wire",
                Description = "Basic electrical component",
                Category = ItemCategory.BasicPart
            },
            new Item
            {
                Id = "cable",
                Name = "Cable",
                Description = "Electrical transmission component",
                Category = ItemCategory.BasicPart
            },
            new Item
            {
                Id = "concrete",
                Name = "Concrete",
                Description = "Basic construction material",
                Category = ItemCategory.BasicPart
            },
            
            // Intermediate Parts
            new Item
            {
                Id = "reinforced_iron_plate",
                Name = "Reinforced Iron Plate",
                Description = "Strengthened iron plate for advanced construction",
                Category = ItemCategory.IntermediatePart
            },
            new Item
            {
                Id = "rotor",
                Name = "Rotor",
                Description = "Rotating component for machines",
                Category = ItemCategory.IntermediatePart
            },
            new Item
            {
                Id = "modular_frame",
                Name = "Modular Frame",
                Description = "Structural component for complex machinery",
                Category = ItemCategory.IntermediatePart
            },
            
            // Advanced Parts
            new Item
            {
                Id = "motor",
                Name = "Motor",
                Description = "Motorized component for advanced machinery",
                Category = ItemCategory.AdvancedPart
            },
            new Item
            {
                Id = "computer",
                Name = "Computer",
                Description = "Advanced computational component",
                Category = ItemCategory.AdvancedPart
            },
            
            // Space Elevator Parts
            new Item
            {
                Id = "smart_plating",
                Name = "Smart Plating",
                Description = "Space Elevator construction component",
                Category = ItemCategory.SpaceElevator
            },
            new Item
            {
                Id = "versatile_framework",
                Name = "Versatile Framework",
                Description = "Space Elevator construction component",
                Category = ItemCategory.SpaceElevator
            },
            new Item
            {
                Id = "automated_wiring",
                Name = "Automated Wiring",
                Description = "Space Elevator construction component",
                Category = ItemCategory.SpaceElevator
            },
            new Item
            {
                Id = "modular_engine",
                Name = "Modular Engine",
                Description = "Space Elevator construction component",
                Category = ItemCategory.SpaceElevator
            }
        };
    }
}