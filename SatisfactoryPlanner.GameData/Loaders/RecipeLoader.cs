using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads recipes from JSON and converts them to domain models
/// </summary>
public class RecipeLoader
{
    private readonly JsonDataLoader<List<RecipeDto>> _jsonLoader;
    private readonly ItemLoader _itemLoader;
    private List<Recipe>? _cachedRecipes;

    public RecipeLoader(string gameDataDirectory, ItemLoader itemLoader)
    {
        var recipesFilePath = Path.Combine(gameDataDirectory, "recipes.json");
        _jsonLoader = new JsonDataLoader<List<RecipeDto>>(recipesFilePath);
        _itemLoader = itemLoader;
    }

    /// <summary>
    /// Loads recipes and converts them to domain models
    /// </summary>
    public async Task<List<Recipe>> LoadRecipesAsync()
    {
        if (_cachedRecipes != null)
            return _cachedRecipes;

        var recipeDtos = await _jsonLoader.LoadAsync();
        var itemLookup = await _itemLoader.LoadItemsLookupAsync();
        
        _cachedRecipes = recipeDtos.Select(dto => ConvertToRecipe(dto, itemLookup)).ToList();
        return _cachedRecipes;
    }

    /// <summary>
    /// Loads recipes and returns them as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Recipe>> LoadRecipesLookupAsync()
    {
        var recipes = await LoadRecipesAsync();
        return recipes.ToDictionary(r => r.Id, r => r);
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