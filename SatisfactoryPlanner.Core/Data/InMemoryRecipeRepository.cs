using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of recipe repository with Satisfactory game data
/// </summary>
public class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly List<Recipe> _recipes;

    public InMemoryRecipeRepository()
    {
        _recipes = InitializeRecipes();
    }

    public Task<List<Recipe>> GetAllRecipesAsync()
    {
        return Task.FromResult(_recipes);
    }

    public Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        return Task.FromResult(_recipes.FirstOrDefault(r => r.Id == id));
    }

    public Task<List<Recipe>> GetRecipesForOutputAsync(string itemId)
    {
        return Task.FromResult(_recipes.Where(r => r.Outputs.Any(o => o.Item.Id == itemId)).ToList());
    }

    public Task<List<Recipe>> GetRecipesForInputAsync(string itemId)
    {
        return Task.FromResult(_recipes.Where(r => r.Inputs.Any(i => i.Item.Id == itemId)).ToList());
    }

    public Task<List<Recipe>> GetRecipesByTierAsync(int maxTier)
    {
        return Task.FromResult(_recipes.Where(r => r.UnlockTier <= maxTier).ToList());
    }

    public Task<List<Recipe>> GetAlternateRecipesAsync()
    {
        return Task.FromResult(_recipes.Where(r => r.IsAlternate).ToList());
    }

    private List<Recipe> InitializeRecipes()
    {
        // Initialize with some basic Satisfactory recipes
        // This would typically be loaded from a data file or database
        var items = GetSampleItems();
        
        return new List<Recipe>
        {
            // Iron Ingot (Smelter)
            new Recipe
            {
                Id = "iron_ingot",
                Name = "Iron Ingot",
                Description = "Smelts Iron Ore into Iron Ingots",
                Inputs = new List<ItemQuantity> { new(items["iron_ore"], 1) },
                Outputs = new List<ItemQuantity> { new(items["iron_ingot"], 1) },
                ProductionTimeSeconds = 2.0,
                CompatibleMachineIds = new List<string> { "smelter" },
                UnlockTier = 0
            },
            
            // Iron Plate (Constructor)
            new Recipe
            {
                Id = "iron_plate",
                Name = "Iron Plate",
                Description = "Constructs Iron Plates from Iron Ingots",
                Inputs = new List<ItemQuantity> { new(items["iron_ingot"], 3) },
                Outputs = new List<ItemQuantity> { new(items["iron_plate"], 2) },
                ProductionTimeSeconds = 6.0,
                CompatibleMachineIds = new List<string> { "constructor" },
                UnlockTier = 0
            },
            
            // Iron Rod (Constructor)
            new Recipe
            {
                Id = "iron_rod",
                Name = "Iron Rod",
                Description = "Constructs Iron Rods from Iron Ingots",
                Inputs = new List<ItemQuantity> { new(items["iron_ingot"], 1) },
                Outputs = new List<ItemQuantity> { new(items["iron_rod"], 1) },
                ProductionTimeSeconds = 4.0,
                CompatibleMachineIds = new List<string> { "constructor" },
                UnlockTier = 0
            },
            
            // Reinforced Iron Plate (Assembler)
            new Recipe
            {
                Id = "reinforced_iron_plate",
                Name = "Reinforced Iron Plate",
                Description = "Assembles Reinforced Iron Plates",
                Inputs = new List<ItemQuantity> 
                { 
                    new(items["iron_plate"], 6),
                    new(items["screw"], 12)
                },
                Outputs = new List<ItemQuantity> { new(items["reinforced_iron_plate"], 1) },
                ProductionTimeSeconds = 12.0,
                CompatibleMachineIds = new List<string> { "assembler" },
                UnlockTier = 0
            },
            
            // Screw (Constructor)
            new Recipe
            {
                Id = "screw",
                Name = "Screw",
                Description = "Constructs Screws from Iron Rods",
                Inputs = new List<ItemQuantity> { new(items["iron_rod"], 1) },
                Outputs = new List<ItemQuantity> { new(items["screw"], 4) },
                ProductionTimeSeconds = 6.0,
                CompatibleMachineIds = new List<string> { "constructor" },
                UnlockTier = 0
            },
            
            // Copper Ingot (Smelter)
            new Recipe
            {
                Id = "copper_ingot",
                Name = "Copper Ingot",
                Description = "Smelts Copper Ore into Copper Ingots",
                Inputs = new List<ItemQuantity> { new(items["copper_ore"], 1) },
                Outputs = new List<ItemQuantity> { new(items["copper_ingot"], 1) },
                ProductionTimeSeconds = 2.0,
                CompatibleMachineIds = new List<string> { "smelter" },
                UnlockTier = 0
            },
            
            // Wire (Constructor)
            new Recipe
            {
                Id = "wire",
                Name = "Wire",
                Description = "Constructs Wire from Copper Ingots",
                Inputs = new List<ItemQuantity> { new(items["copper_ingot"], 1) },
                Outputs = new List<ItemQuantity> { new(items["wire"], 2) },
                ProductionTimeSeconds = 4.0,
                CompatibleMachineIds = new List<string> { "constructor" },
                UnlockTier = 0
            },
            
            // Cable (Constructor)
            new Recipe
            {
                Id = "cable",
                Name = "Cable",
                Description = "Constructs Cables from Wire",
                Inputs = new List<ItemQuantity> { new(items["wire"], 2) },
                Outputs = new List<ItemQuantity> { new(items["cable"], 1) },
                ProductionTimeSeconds = 2.0,
                CompatibleMachineIds = new List<string> { "constructor" },
                UnlockTier = 0
            }
        };
    }

    private Dictionary<string, Item> GetSampleItems()
    {
        return new Dictionary<string, Item>
        {
            ["iron_ore"] = new Item { Id = "iron_ore", Name = "Iron Ore", Category = ItemCategory.RawResource, IsRawResource = true },
            ["copper_ore"] = new Item { Id = "copper_ore", Name = "Copper Ore", Category = ItemCategory.RawResource, IsRawResource = true },
            ["iron_ingot"] = new Item { Id = "iron_ingot", Name = "Iron Ingot", Category = ItemCategory.Ingot },
            ["copper_ingot"] = new Item { Id = "copper_ingot", Name = "Copper Ingot", Category = ItemCategory.Ingot },
            ["iron_plate"] = new Item { Id = "iron_plate", Name = "Iron Plate", Category = ItemCategory.BasicPart },
            ["iron_rod"] = new Item { Id = "iron_rod", Name = "Iron Rod", Category = ItemCategory.BasicPart },
            ["screw"] = new Item { Id = "screw", Name = "Screw", Category = ItemCategory.BasicPart },
            ["wire"] = new Item { Id = "wire", Name = "Wire", Category = ItemCategory.BasicPart },
            ["cable"] = new Item { Id = "cable", Name = "Cable", Category = ItemCategory.BasicPart },
            ["reinforced_iron_plate"] = new Item { Id = "reinforced_iron_plate", Name = "Reinforced Iron Plate", Category = ItemCategory.IntermediatePart }
        };
    }
}