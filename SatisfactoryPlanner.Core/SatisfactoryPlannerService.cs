using SatisfactoryPlanner.Core.Data;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core;

/// <summary>
/// Main service facade for the Satisfactory production planner
/// </summary>
public class SatisfactoryPlannerService
{
    private readonly ProductionGraphBuilder _graphBuilder;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IItemRepository _itemRepository;

    public SatisfactoryPlannerService()
    {
        // Initialize with file-based repositories
        var dataFilePath = GetDefaultDataFilePath();
        _recipeRepository = new FileBasedRecipeRepository(dataFilePath);
        _machineRepository = new InMemoryMachineRepository(dataFilePath);
        _itemRepository = new InMemoryItemRepository(dataFilePath);
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _machineRepository);
    }

    public SatisfactoryPlannerService(string dataFilePath)
    {
        // Initialize with custom data file
        _recipeRepository = new FileBasedRecipeRepository(dataFilePath);
        _machineRepository = new InMemoryMachineRepository(dataFilePath);
        _itemRepository = new InMemoryItemRepository(dataFilePath);
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _machineRepository);
    }

    public SatisfactoryPlannerService(
        IRecipeRepository recipeRepository,
        IMachineRepository machineRepository,
        IItemRepository itemRepository)
    {
        _recipeRepository = recipeRepository;
        _machineRepository = machineRepository;
        _itemRepository = itemRepository;
        _graphBuilder = new ProductionGraphBuilder(_recipeRepository, _machineRepository);
    }

    /// <summary>
    /// Gets the default path to the game data file
    /// </summary>
    private static string GetDefaultDataFilePath()
    {
        // Try to find the data file relative to the assembly location
        var assemblyDir = Path.GetDirectoryName(typeof(SatisfactoryPlannerService).Assembly.Location);
        if (assemblyDir != null)
        {
            var dataPath = Path.Combine(assemblyDir, "Data", "GameData.json");
            if (File.Exists(dataPath))
                return dataPath;
        }

        // Try relative to current directory (development scenario)
        var currentDirPath = Path.Combine("Data", "GameData.json");
        if (File.Exists(currentDirPath))
            return currentDirPath;

        // Try in the Core project directory (for testing)
        var coreProjectPath = Path.Combine("..", "..", "..", "SatisfactoryPlanner.Core", "Data", "GameData.json");
        if (File.Exists(coreProjectPath))
            return Path.GetFullPath(coreProjectPath);

        throw new FileNotFoundException("Could not find GameData.json file. Please ensure it exists in the Data directory.");
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
    /// Gets all available machines for a specific tier
    /// </summary>
    public async Task<List<Machine>> GetAvailableMachinesAsync(int gameTier)
    {
        return await _machineRepository.GetMachinesByTierAsync(gameTier);
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
            TotalMachines = graph.Nodes.Sum(n => (int)Math.Ceiling(n.MachineCount)),
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
                       * output.MachineCount * output.ClockSpeed;
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