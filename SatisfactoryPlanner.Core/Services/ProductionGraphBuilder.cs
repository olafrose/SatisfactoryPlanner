using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Services;

/// <summary>
/// Service for building and optimizing production graphs
/// </summary>
public class ProductionGraphBuilder
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMachineRepository _machineRepository;

    public ProductionGraphBuilder(IRecipeRepository recipeRepository, IMachineRepository machineRepository)
    {
        _recipeRepository = recipeRepository;
        _machineRepository = machineRepository;
    }

    /// <summary>
    /// Builds a production graph to produce the specified target items
    /// </summary>
    /// <param name="targetOutputs">Items and quantities to produce per minute</param>
    /// <param name="gameTier">Game tier to limit available recipes and machines</param>
    /// <param name="options">Build options for optimization preferences</param>
    /// <returns>Complete production graph</returns>
    public async Task<ProductionGraph> BuildProductionGraphAsync(
        List<ItemQuantity> targetOutputs, 
        int gameTier, 
        ProductionGraphOptions? options = null)
    {
        options ??= new ProductionGraphOptions();
        
        var graph = new ProductionGraph
        {
            Name = $"Production for {string.Join(", ", targetOutputs.Select(t => t.Item.Name))}",
            TargetOutputs = targetOutputs,
            GameTier = gameTier
        };

        // Build the graph recursively for each target output
        foreach (var target in targetOutputs)
        {
            var leafNode = await BuildProductionChainAsync(target, gameTier, options, graph);
            if (leafNode != null)
            {
                graph.Nodes.Add(leafNode);
            }
        }

        // Optimize the graph
        OptimizeGraph(graph, options);

        return graph;
    }

    /// <summary>
    /// Builds a production graph to produce the specified target items with player research state
    /// </summary>
    /// <param name="targetOutputs">Items and quantities to produce per minute</param>
    /// <param name="playerState">Player's research state and preferences</param>
    /// <param name="options">Build options for optimization preferences</param>
    /// <returns>Complete production graph</returns>
    public async Task<ProductionGraph> BuildProductionGraphAsync(
        List<ItemQuantity> targetOutputs, 
        PlayerResearchState playerState, 
        ProductionGraphOptions? options = null)
    {
        options ??= new ProductionGraphOptions();
        
        // Configure options based on player preferences
        options.OptimizeFor = playerState.PreferredOptimization;
        options.AllowOverclocking = playerState.AllowOverclocking;
        options.MaxOverclockPercentage = playerState.MaxOverclockPercentage;
        
        var graph = new ProductionGraph
        {
            Name = $"Production for {string.Join(", ", targetOutputs.Select(t => t.Item.Name))}",
            TargetOutputs = targetOutputs,
            GameTier = playerState.CurrentTier
        };

        // Build the graph recursively for each target output
        foreach (var target in targetOutputs)
        {
            var leafNode = await BuildProductionChainAsync(target, playerState, options, graph);
            if (leafNode != null)
            {
                graph.Nodes.Add(leafNode);
            }
        }

        // Optimize the graph
        OptimizeGraph(graph, options);

        return graph;
    }

    /// <summary>
    /// Recursively builds the production chain for a specific item
    /// </summary>
    private async Task<ProductionNode?> BuildProductionChainAsync(
        ItemQuantity targetOutput,
        int gameTier,
        ProductionGraphOptions options,
        ProductionGraph graph)
    {
        // If it's a raw resource, no production node needed
        if (targetOutput.Item.IsRawResource)
        {
            return null;
        }

        // Find the best recipe for this item
        var availableRecipes = await _recipeRepository.GetRecipesForOutputAsync(targetOutput.Item.Id);
        // Note: Recipe filtering by milestones should be handled at a higher level
        var validRecipes = availableRecipes.ToList();
        
        if (!validRecipes.Any())
        {
            throw new InvalidOperationException($"No recipes available for {targetOutput.Item.Name} at tier {gameTier}");
        }

        var selectedRecipe = SelectBestRecipe(validRecipes, options);
        
        // Find the best machine for this recipe
        var availableMachines = await _machineRepository.GetMachinesForRecipeAsync(selectedRecipe.Id);
        // Note: Machine filtering by milestones should be handled at a higher level
        var validMachines = availableMachines.ToList();
        
        if (!validMachines.Any())
        {
            throw new InvalidOperationException($"No machines available for recipe {selectedRecipe.Name} at tier {gameTier}");
        }

        var selectedMachine = SelectBestMachine(validMachines, options);

        // Create the production node
        var node = new ProductionNode
        {
            Recipe = selectedRecipe,
            Machine = selectedMachine,
            TargetProductionRate = targetOutput.QuantityPerMinute
        };

        // Calculate required machine count
        var primaryOutput = selectedRecipe.Outputs.First(o => o.Item.Id == targetOutput.Item.Id);
        var baseProductionRate = (primaryOutput.Quantity * 60.0) / selectedRecipe.ProductionTimeSeconds;
        var adjustedRate = baseProductionRate * selectedMachine.ProductionSpeed;
        
        node.MachineCount = Math.Ceiling(targetOutput.QuantityPerMinute / adjustedRate);

        // Build input chains recursively
        foreach (var input in selectedRecipe.Inputs)
        {
            var requiredInputRate = (input.Quantity * 60.0 / selectedRecipe.ProductionTimeSeconds) * node.MachineCount;
            var inputTarget = new ItemQuantity(input.Item, requiredInputRate) { QuantityPerMinute = requiredInputRate };
            
            var inputNode = await BuildProductionChainAsync(inputTarget, gameTier, options, graph);
            if (inputNode != null)
            {
                node.InputNodes.Add(inputNode);
                inputNode.OutputNodes.Add(node);
                
                // Add to graph if not already present
                if (!graph.Nodes.Contains(inputNode))
                {
                    graph.Nodes.Add(inputNode);
                }
            }
        }

        return node;
    }

    /// <summary>
    /// Recursively builds the production chain for a specific item with player research state
    /// </summary>
    private async Task<ProductionNode?> BuildProductionChainAsync(
        ItemQuantity targetOutput,
        PlayerResearchState playerState,
        ProductionGraphOptions options,
        ProductionGraph graph)
    {
        // If it's a raw resource, no production node needed
        if (targetOutput.Item.IsRawResource)
        {
            return null;
        }

        // Find the best recipe for this item
        var availableRecipes = await _recipeRepository.GetRecipesForOutputAsync(targetOutput.Item.Id);
        var validRecipes = availableRecipes.Where(r => playerState.IsRecipeAvailable(r)).ToList();
        
        if (!validRecipes.Any())
        {
            throw new InvalidOperationException($"No recipes available for {targetOutput.Item.Name} at tier {playerState.CurrentTier}");
        }

        var selectedRecipe = SelectBestRecipe(validRecipes, options);
        
        // Find the best machine for this recipe
        var availableMachines = await _machineRepository.GetMachinesForRecipeAsync(selectedRecipe.Id);
        // Note: Machine filtering by milestones should be handled at a higher level
        var validMachines = availableMachines.ToList();
        
        if (!validMachines.Any())
        {
            throw new InvalidOperationException($"No machines available for recipe {selectedRecipe.Name} at tier {playerState.CurrentTier}");
        }

        var selectedMachine = SelectBestMachine(validMachines, options);

        // Create the production node
        var node = new ProductionNode
        {
            Recipe = selectedRecipe,
            Machine = selectedMachine,
            TargetProductionRate = targetOutput.QuantityPerMinute
        };

        // Calculate required machine count
        var primaryOutput = selectedRecipe.Outputs.First(o => o.Item.Id == targetOutput.Item.Id);
        var baseProductionRate = (primaryOutput.Quantity * 60.0) / selectedRecipe.ProductionTimeSeconds;
        var adjustedRate = baseProductionRate * selectedMachine.ProductionSpeed;
        
        node.MachineCount = Math.Ceiling(targetOutput.QuantityPerMinute / adjustedRate);

        // Build input chains recursively
        foreach (var input in selectedRecipe.Inputs)
        {
            var requiredInputRate = (input.Quantity * 60.0 / selectedRecipe.ProductionTimeSeconds) * node.MachineCount;
            var inputTarget = new ItemQuantity(input.Item, requiredInputRate) { QuantityPerMinute = requiredInputRate };
            
            var inputNode = await BuildProductionChainAsync(inputTarget, playerState, options, graph);
            if (inputNode != null)
            {
                node.InputNodes.Add(inputNode);
                inputNode.OutputNodes.Add(node);
                
                // Add to graph if not already present
                if (!graph.Nodes.Contains(inputNode))
                {
                    graph.Nodes.Add(inputNode);
                }
            }
        }

        return node;
    }

    /// <summary>
    /// Selects the best recipe based on the optimization options and available recipes
    /// </summary>
    private Recipe SelectBestRecipe(List<Recipe> recipes, ProductionGraphOptions options)
    {
        if (options.PreferAlternateRecipes)
        {
            // Prefer alternate recipes first
            return recipes.OrderByDescending(r => r.IsAlternate).ThenBy(r => r.Inputs.Count).First();
        }

        // For efficiency optimization, prefer recipes with better resource efficiency
        if (options.OptimizeFor == OptimizationTarget.ResourceEfficiency)
        {
            return recipes
                .OrderByDescending(r => r.IsAlternate) // Prefer alternates for efficiency
                .ThenBy(r => CalculateResourceEfficiency(r))
                .First();
        }

        // For speed optimization, prefer recipes with higher output rates
        if (options.OptimizeFor == OptimizationTarget.Speed)
        {
            return recipes
                .OrderByDescending(r => r.Outputs.Sum(o => o.Quantity) / r.ProductionTimeSeconds)
                .First();
        }

        // Default: prefer standard recipes, then by simplicity
        return recipes.OrderBy(r => r.IsAlternate).ThenBy(r => r.Inputs.Count).First();
    }

    /// <summary>
    /// Calculates resource efficiency score (higher is better)
    /// </summary>
    private double CalculateResourceEfficiency(Recipe recipe)
    {
        var totalInputs = recipe.Inputs.Sum(i => i.Quantity);
        var totalOutputs = recipe.Outputs.Sum(o => o.Quantity);
        return totalOutputs / Math.Max(totalInputs, 1.0);
    }

    /// <summary>
    /// Selects the best machine based on the optimization options
    /// </summary>
    private Machine SelectBestMachine(List<Machine> machines, ProductionGraphOptions options)
    {
        return options.OptimizeFor switch
        {
            OptimizationTarget.PowerEfficiency => machines.OrderBy(m => m.PowerConsumption / m.ProductionSpeed).First(),
            OptimizationTarget.Speed => machines.OrderByDescending(m => m.ProductionSpeed).First(),
            OptimizationTarget.Simplicity => machines.OrderBy(m => m.PowerConsumption).First(),
            _ => machines.First()
        };
    }

    /// <summary>
    /// Optimizes the production graph for efficiency
    /// </summary>
    private void OptimizeGraph(ProductionGraph graph, ProductionGraphOptions options)
    {
        // Merge duplicate nodes producing the same item
        MergeDuplicateNodes(graph);
        
        // Balance production rates to minimize waste
        BalanceProductionRates(graph);
        
        // Optimize overclocking if enabled
        if (options.AllowOverclocking)
        {
            OptimizeOverclocking(graph, options);
        }
    }

    private void MergeDuplicateNodes(ProductionGraph graph)
    {
        // Group nodes by recipe and machine type
        var nodeGroups = graph.Nodes
            .GroupBy(n => new { RecipeId = n.Recipe.Id, MachineId = n.Machine.Id })
            .Where(g => g.Count() > 1);

        foreach (var group in nodeGroups)
        {
            var primaryNode = group.First();
            var duplicateNodes = group.Skip(1).ToList();

            // Merge machine counts
            primaryNode.MachineCount += duplicateNodes.Sum(n => n.MachineCount);
            
            // Update connections
            foreach (var duplicate in duplicateNodes)
            {
                foreach (var outputNode in duplicate.OutputNodes)
                {
                    outputNode.InputNodes.Remove(duplicate);
                    if (!outputNode.InputNodes.Contains(primaryNode))
                    {
                        outputNode.InputNodes.Add(primaryNode);
                        primaryNode.OutputNodes.Add(outputNode);
                    }
                }
                
                graph.Nodes.Remove(duplicate);
            }
        }
    }

    private void BalanceProductionRates(ProductionGraph graph)
    {
        // Traverse the graph and balance production rates
        foreach (var node in graph.Nodes.OrderBy(n => n.InputNodes.Count))
        {
            if (node.InputNodes.Any())
            {
                // Adjust input node production to match consumption
                foreach (var inputNode in node.InputNodes)
                {
                    var requiredInputItems = node.Recipe.Inputs
                        .Where(i => inputNode.Recipe.Outputs.Any(o => o.Item.Id == i.Item.Id));
                    
                    foreach (var requiredInput in requiredInputItems)
                    {
                        var requiredRate = (requiredInput.Quantity * 60.0 / node.Recipe.ProductionTimeSeconds) 
                                         * node.MachineCount * node.ClockSpeed;
                        
                        var outputFromInput = inputNode.Recipe.Outputs
                            .First(o => o.Item.Id == requiredInput.Item.Id);
                        var currentRate = (outputFromInput.Quantity * 60.0 / inputNode.Recipe.ProductionTimeSeconds) 
                                        * inputNode.MachineCount * inputNode.ClockSpeed;
                        
                        if (currentRate < requiredRate)
                        {
                            var scaleFactor = requiredRate / currentRate;
                            inputNode.MachineCount *= scaleFactor;
                        }
                    }
                }
            }
        }
    }

    private void OptimizeOverclocking(ProductionGraph graph, ProductionGraphOptions options)
    {
        foreach (var node in graph.Nodes.Where(n => n.Machine.CanOverclock))
        {
            // Try to reduce machine count by overclocking
            var maxOverclock = options.MaxOverclockPercentage / 100.0;
            var potentialClockSpeed = Math.Min(maxOverclock, 2.5); // Game limit is 250%
            
            var newMachineCount = node.MachineCount / potentialClockSpeed;
            
            // Only overclock if it results in fewer machines
            if (Math.Ceiling(newMachineCount) < Math.Ceiling(node.MachineCount))
            {
                node.MachineCount = Math.Ceiling(newMachineCount);
                node.ClockSpeed = potentialClockSpeed;
            }
        }
    }
}

/// <summary>
/// Options for building production graphs
/// </summary>
public class ProductionGraphOptions
{
    public OptimizationTarget OptimizeFor { get; set; } = OptimizationTarget.Simplicity;
    public bool PreferAlternateRecipes { get; set; } = false;
    public bool AllowOverclocking { get; set; } = true;
    public double MaxOverclockPercentage { get; set; } = 250.0; // 250% max in game
    public bool MinimizePowerConsumption { get; set; } = false;
    public bool AllowWaste { get; set; } = false; // Allow overproduction if it simplifies the build
}

/// <summary>
/// Optimization targets for production graphs
/// </summary>
public enum OptimizationTarget
{
    PowerEfficiency,  // Minimize power consumption
    Speed,           // Maximize production speed
    Simplicity,      // Minimize complexity (fewer machine types, fewer nodes)
    ResourceEfficiency // Minimize raw resource consumption
}