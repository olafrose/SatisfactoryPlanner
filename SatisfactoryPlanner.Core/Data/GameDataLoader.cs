using System.Text.Json;
using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Loads game data from JSON files
/// </summary>
public class GameDataLoader
{
    private readonly string _dataFilePath;
    private GameDataDto? _cachedData;

    public GameDataLoader(string dataFilePath)
    {
        _dataFilePath = dataFilePath;
    }

    /// <summary>
    /// Loads and parses the game data from the JSON file
    /// </summary>
    public async Task<GameDataDto> LoadGameDataAsync()
    {
        if (_cachedData != null)
            return _cachedData;

        try
        {
            var jsonContent = await File.ReadAllTextAsync(_dataFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            _cachedData = JsonSerializer.Deserialize<GameDataDto>(jsonContent, options);
            
            if (_cachedData == null)
                throw new InvalidOperationException("Failed to deserialize game data");

            return _cachedData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"Game data file not found: {_dataFilePath}");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON in game data file: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Converts DTOs to domain models with proper relationships
    /// </summary>
    public async Task<(List<Item> items, List<Recipe> recipes)> LoadModelsAsync()
    {
        var gameData = await LoadGameDataAsync();
        
        // Convert items first
        var items = gameData.Items.Select(ConvertToItem).ToList();
        var itemLookup = items.ToDictionary(i => i.Id, i => i);

        // Convert recipes with item references
        var recipes = gameData.Recipes.Select(dto => ConvertToRecipe(dto, itemLookup)).ToList();

        return (items, recipes);
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

    private static Recipe ConvertToRecipe(RecipeDto dto, Dictionary<string, Item> itemLookup)
    {
        var recipe = new Recipe
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            ProductionTimeSeconds = dto.ProductionTimeSeconds,
            CompatibleMachineIds = dto.CompatibleMachineIds,
            UnlockTier = dto.UnlockTier,
            IsAlternate = dto.IsAlternate
        };

        // Convert inputs
        foreach (var inputDto in dto.Inputs)
        {
            if (itemLookup.TryGetValue(inputDto.ItemId, out var item))
            {
                recipe.Inputs.Add(new ItemQuantity(item, inputDto.Quantity));
            }
            else
            {
                throw new InvalidOperationException($"Recipe '{dto.Id}' references unknown input item '{inputDto.ItemId}'");
            }
        }

        // Convert outputs
        foreach (var outputDto in dto.Outputs)
        {
            if (itemLookup.TryGetValue(outputDto.ItemId, out var item))
            {
                recipe.Outputs.Add(new ItemQuantity(item, outputDto.Quantity));
            }
            else
            {
                throw new InvalidOperationException($"Recipe '{dto.Id}' references unknown output item '{outputDto.ItemId}'");
            }
        }

        return recipe;
    }

    /// <summary>
    /// Refreshes the cached data (useful for hot-reloading during development)
    /// </summary>
    public void ClearCache()
    {
        _cachedData = null;
    }
}