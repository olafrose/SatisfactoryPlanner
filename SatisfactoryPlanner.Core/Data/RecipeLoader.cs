using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Loads recipes from JSON and converts them to domain models
/// </summary>
public class RecipeLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private readonly ItemLoader _itemLoader;
    private List<Recipe>? _cachedRecipes;

    public RecipeLoader(string filePath, ItemLoader itemLoader)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
        _itemLoader = itemLoader;
    }

    /// <summary>
    /// Loads recipes and converts them to domain models
    /// </summary>
    public async Task<List<Recipe>> LoadRecipesAsync()
    {
        if (_cachedRecipes != null)
            return _cachedRecipes;

        var gameData = await _jsonLoader.LoadAsync();
        var itemLookup = await _itemLoader.LoadItemsLookupAsync();
        
        _cachedRecipes = gameData.Recipes.Select(dto => ConvertToRecipe(dto, itemLookup)).ToList();
        return _cachedRecipes;
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
    /// Clears the cached data
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedRecipes = null;
    }
}