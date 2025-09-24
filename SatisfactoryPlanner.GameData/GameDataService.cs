using SatisfactoryPlanner.GameData.Models;
using SatisfactoryPlanner.GameData.Loaders;
using SatisfactoryPlanner.GameData.Services;

namespace SatisfactoryPlanner.GameData;

/// <summary>
/// Main service for loading and accessing Satisfactory game data
/// </summary>
public class GameDataService
{
    private readonly string _gameDataPath;
    private readonly ItemLoader _itemLoader;
    private readonly MachineLoader _machineLoader;
    private readonly BuildingLoader _buildingLoader;
    private readonly RecipeLoader _recipeLoader;
    private readonly MilestoneLoader _milestoneLoader;
    private readonly IconService _iconService;

    public GameDataService(string gameDataDirectory)
    {
        _gameDataPath = gameDataDirectory ?? throw new ArgumentNullException(nameof(gameDataDirectory));
        
        if (!Directory.Exists(_gameDataPath))
            throw new DirectoryNotFoundException($"GameData directory not found: {_gameDataPath}");
        
        // Initialize loaders with the directory containing individual JSON files
        _itemLoader = new ItemLoader(_gameDataPath);
        _machineLoader = new MachineLoader(_gameDataPath);
        _buildingLoader = new BuildingLoader(_gameDataPath);
        _recipeLoader = new RecipeLoader(_gameDataPath, _itemLoader);
        _milestoneLoader = new MilestoneLoader(_gameDataPath, _itemLoader);
        _iconService = new IconService(_gameDataPath);
    }

    /// <summary>
    /// Gets the icon service for loading game icons
    /// </summary>
    public IconService Icons => _iconService;

    /// <summary>
    /// Loads all items from the game data
    /// </summary>
    public async Task<List<Item>> LoadItemsAsync() => await _itemLoader.LoadItemsAsync();

    /// <summary>
    /// Loads items as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Item>> LoadItemsLookupAsync() => await _itemLoader.LoadItemsLookupAsync();

    /// <summary>
    /// Loads all buildings from the game data
    /// </summary>
    public async Task<List<Building>> LoadBuildingsAsync() => await _buildingLoader.LoadBuildingsAsync();

    /// <summary>
    /// Loads buildings as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Building>> LoadBuildingsLookupAsync() => await _buildingLoader.LoadBuildingsLookupAsync();

    /// <summary>
    /// Loads all machines from the game data (obsolete - use LoadBuildingsAsync)
    /// </summary>
    [Obsolete("Use LoadBuildingsAsync instead of LoadMachinesAsync to align with wiki terminology")]
    public async Task<List<Machine>> LoadMachinesAsync() => await _machineLoader.LoadMachinesAsync();

    /// <summary>
    /// Loads machines as a lookup dictionary by ID (obsolete - use LoadBuildingsLookupAsync)
    /// </summary>
    [Obsolete("Use LoadBuildingsLookupAsync instead of LoadMachinesLookupAsync to align with wiki terminology")]
    public async Task<Dictionary<string, Machine>> LoadMachinesLookupAsync() => await _machineLoader.LoadMachinesLookupAsync();

    /// <summary>
    /// Loads all recipes from the game data
    /// </summary>
    public async Task<List<Recipe>> LoadRecipesAsync() => await _recipeLoader.LoadRecipesAsync();

    /// <summary>
    /// Loads recipes as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Recipe>> LoadRecipesLookupAsync() => await _recipeLoader.LoadRecipesLookupAsync();

    /// <summary>
    /// Loads all milestones from the game data
    /// </summary>
    public async Task<List<Milestone>> LoadMilestonesAsync() => await _milestoneLoader.LoadMilestonesAsync();

    /// <summary>
    /// Loads milestones as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Milestone>> LoadMilestonesLookupAsync() => await _milestoneLoader.LoadMilestonesLookupAsync();

    /// <summary>
    /// Loads all game data at once
    /// </summary>
    public async Task<GameData> LoadAllDataAsync()
    {
        var items = await LoadItemsAsync();
        var buildings = await LoadBuildingsAsync();
        var recipes = await LoadRecipesAsync();
        var milestones = await LoadMilestonesAsync();

        return new GameData
        {
            Items = items,
            Buildings = buildings,
            Recipes = recipes,
            Milestones = milestones
        };
    }

    /// <summary>
    /// Clears all cached data
    /// </summary>
    public void ClearCache()
    {
        _itemLoader.ClearCache();
        _machineLoader.ClearCache();
        _recipeLoader.ClearCache();
        _milestoneLoader.ClearCache();
        _iconService.ClearCache();
    }
}

/// <summary>
/// Container for all game data
/// </summary>
public class GameData
{
    public List<Item> Items { get; set; } = new();
    public List<Building> Buildings { get; set; } = new();
    
    /// <summary>
    /// Machines property for backward compatibility (obsolete - use Buildings)
    /// </summary>
    [Obsolete("Use Buildings instead of Machines to align with wiki terminology")]
    public List<Machine> Machines 
    { 
        get => Buildings.Cast<Machine>().ToList(); 
        set => Buildings = value.Cast<Building>().ToList(); 
    }
    
    public List<Recipe> Recipes { get; set; } = new();
    public List<Milestone> Milestones { get; set; } = new();
}