using SatisfactoryPlanner.Core.Data;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Models;
using GameDataItem = SatisfactoryPlanner.GameData.Models.Item;
using GameDataItemQuantity = SatisfactoryPlanner.GameData.Models.ItemQuantity;

namespace SatisfactoryPlanner.Core;

/// <summary>
/// Main service facade for the Satisfactory production planner
/// </summary>
public class SatisfactoryPlannerService
{
    private readonly ProductionGraphBuilder _graphBuilder;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IItemRepository _itemRepository;

    public SatisfactoryPlannerService()
    {
        // Initialize with consistent repositories
        var gameDataDirectory = GetDefaultGameDataDirectory();
        var gameDataService = new GameDataService(gameDataDirectory);
        _recipeRepository = new InMemoryRecipeRepository(gameDataService);
        _buildingRepository = new InMemoryBuildingRepository(gameDataService);
        _itemRepository = new InMemoryItemRepository(gameDataService);
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _buildingRepository);
    }

    public SatisfactoryPlannerService(string gameDataDirectory)
    {
        // Initialize with custom data directory
        var gameDataService = new GameDataService(gameDataDirectory);
        _recipeRepository = new InMemoryRecipeRepository(gameDataService);
        _buildingRepository = new InMemoryBuildingRepository(gameDataService);
        _itemRepository = new InMemoryItemRepository(gameDataService);
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _buildingRepository);
    }

    public SatisfactoryPlannerService(
        IRecipeRepository recipeRepository,
        IBuildingRepository buildingRepository,
        IItemRepository itemRepository)
    {
        _recipeRepository = recipeRepository;
        _buildingRepository = buildingRepository;
        _itemRepository = itemRepository;
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _buildingRepository);
    }

    /// <summary>
    /// Gets the default game data directory path
    /// </summary>
    private static string GetDefaultGameDataDirectory()
    {
        // Try to find the data directory relative to the assembly location
        var assemblyDir = Path.GetDirectoryName(typeof(SatisfactoryPlannerService).Assembly.Location);
        if (assemblyDir != null)
        {
            var dataPath = Path.Combine(assemblyDir, "Data", "GameData");
            if (Directory.Exists(dataPath))
                return dataPath;
        }

        // Try relative to current directory (development scenario)
        var currentDirPath = Path.Combine("Data", "GameData");
        if (Directory.Exists(currentDirPath))
            return currentDirPath;

        // Try in the GameData library project directory
        var gameDataProjectPath = Path.Combine("..", "..", "..", "SatisfactoryPlanner.GameData", "Data", "GameData");
        if (Directory.Exists(gameDataProjectPath))
            return Path.GetFullPath(gameDataProjectPath);

        // Try in the old Core project directory (for backward compatibility)
        var coreProjectPath = Path.Combine("..", "..", "..", "SatisfactoryPlanner.Core", "Data", "GameData");
        if (Directory.Exists(coreProjectPath))
            return Path.GetFullPath(coreProjectPath);

        // If not found, return a default path that can be created
        return Path.Combine("Data", "GameData");
    }

    /// <summary>
    /// Plans a production line for the specified items and quantities
    /// </summary>
    /// <param name="targetItems">Items to produce with their desired rates per minute</param>
    /// <param name="gameTier">Current game tier (0-8)</param>
    /// <param name="options">Build options for optimization</param>
    /// <returns>Complete production graph</returns>
    public async Task<ProductionGraph> PlanProductionAsync(
        Dictionary<string, double> targetItems,
        int gameTier,
        ProductionGraphOptions? options = null)
    {
        var targetOutputs = new List<SatisfactoryPlanner.GameData.Models.ItemQuantity>();
        
        foreach (var (itemId, quantity) in targetItems)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item == null)
            {
                throw new ArgumentException($"Item with ID '{itemId}' not found");
            }
            
            targetOutputs.Add(new ItemQuantity(item, quantity) { QuantityPerMinute = quantity });
        }

        return await _graphBuilder.BuildProductionGraphAsync(targetOutputs, gameTier, options);
    }

    /// <summary>
    /// Plans a production line for the specified items and quantities with player research state
    /// </summary>
    /// <param name="targetItems">Items to produce with their desired rates per minute</param>
    /// <param name="playerState">Player's research state and preferences</param>
    /// <param name="options">Build options for optimization</param>
    /// <returns>Complete production graph</returns>
    public async Task<ProductionGraph> PlanProductionAsync(
        Dictionary<string, double> targetItems,
        PlayerResearchState playerState,
        ProductionGraphOptions? options = null)
    {
        var targetOutputs = new List<ItemQuantity>();
        
        foreach (var (itemId, quantity) in targetItems)
        {
            var item = await _itemRepository.GetItemByIdAsync(itemId);
            if (item == null)
            {
                throw new ArgumentException($"Item with ID '{itemId}' not found");
            }
            
            targetOutputs.Add(new ItemQuantity(item, quantity) { QuantityPerMinute = quantity });
        }

        return await _graphBuilder.BuildProductionGraphAsync(targetOutputs, playerState, options);
    }

    /// <summary>
    /// Gets all available items
    /// </summary>
    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _itemRepository.GetAllItemsAsync();
    }

    /// <summary>
    /// Gets all available buildings for a specific tier
    /// </summary>
    public async Task<List<Building>> GetAvailableBuildingsAsync(int gameTier)
    {
        return await _buildingRepository.GetBuildingsByTierAsync(gameTier);
    }

    /// <summary>
    /// Gets all available machines for a specific tier (obsolete - use GetAvailableBuildingsAsync)
    /// </summary>
    [Obsolete("Use GetAvailableBuildingsAsync instead of GetAvailableMachinesAsync to align with wiki terminology")]
    public async Task<List<Machine>> GetAvailableMachinesAsync(int gameTier)
    {
        var buildings = await _buildingRepository.GetBuildingsByTierAsync(gameTier);
        return buildings.Cast<Machine>().ToList();
    }

    /// <summary>
    /// Gets all available recipes for a specific tier
    /// </summary>
    public async Task<List<Recipe>> GetAvailableRecipesAsync(int gameTier)
    {
        return await _recipeRepository.GetRecipesByTierAsync(gameTier);
    }

    /// <summary>
    /// Analyzes a production graph and provides optimization suggestions
    /// </summary>
    public ProductionAnalysis AnalyzeProduction(ProductionGraph graph)
    {
        return new ProductionAnalysis
        {
            TotalMachines = graph.Nodes.Sum(n => (int)Math.Ceiling(n.BuildingCount)),
            TotalPowerConsumption = graph.TotalPowerConsumption,
            RequiredResources = graph.RequiredResources,
            BottleneckNodes = FindBottlenecks(graph),
            EfficiencyScore = CalculateEfficiencyScore(graph),
            Suggestions = GenerateOptimizationSuggestions(graph)
        };
    }

    private List<ProductionNode> FindBottlenecks(ProductionGraph graph)
    {
        // Find nodes where output doesn't meet demand
        return graph.Nodes.Where(node =>
        {
            var totalDemand = node.OutputNodes.Sum(output =>
            {
                var requiredInput = output.Recipe.Inputs
                    .FirstOrDefault(i => node.Recipe.Outputs.Any(o => o.Item.Id == i.Item.Id));
                if (requiredInput == null) return 0;
                
                return (requiredInput.Quantity * 60.0 / output.Recipe.ProductionTimeSeconds) 
                       * output.BuildingCount * output.ClockSpeed;
            });
            
            return node.ActualProductionRate < totalDemand * 0.98; // 2% tolerance
        }).ToList();
    }

    private double CalculateEfficiencyScore(ProductionGraph graph)
    {
        // Simple efficiency calculation based on power per output
        var totalOutput = graph.LeafNodes.Sum(n => n.ActualProductionRate);
        if (totalOutput == 0) return 0;
        
        var powerEfficiency = totalOutput / graph.TotalPowerConsumption;
        var complexityPenalty = 1.0 - (graph.Nodes.Count * 0.01); // Slight penalty for complexity
        
        return Math.Max(0, powerEfficiency * complexityPenalty);
    }

    private List<string> GenerateOptimizationSuggestions(ProductionGraph graph)
    {
        var suggestions = new List<string>();
        
        var bottlenecks = FindBottlenecks(graph);
        if (bottlenecks.Any())
        {
            suggestions.Add($"Consider increasing production or overclocking for: {string.Join(", ", bottlenecks.Select(b => b.Recipe.Name))}");
        }
        
        var lowEfficiencyNodes = graph.Nodes.Where(n => n.ClockSpeed < 0.8).ToList();
        if (lowEfficiencyNodes.Any())
        {
            suggestions.Add("Some machines are underclocked. Consider reducing machine count and increasing clock speed.");
        }
        
        var highPowerNodes = graph.Nodes.Where(n => n.TotalPowerConsumption > 100).ToList();
        if (highPowerNodes.Any())
        {
            suggestions.Add("High power consumption detected. Consider power generation capacity.");
        }
        
        return suggestions;
    }
}

/// <summary>
/// Analysis results for a production graph
/// </summary>
public class ProductionAnalysis
{
    public int TotalMachines { get; set; }
    public double TotalPowerConsumption { get; set; }
    public List<ItemQuantity> RequiredResources { get; set; } = new();
    public List<ProductionNode> BottleneckNodes { get; set; } = new();
    public double EfficiencyScore { get; set; }
    public List<string> Suggestions { get; set; } = new();
}