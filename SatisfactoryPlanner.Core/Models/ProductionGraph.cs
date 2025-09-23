namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents a production node in the production graph
/// </summary>
public class ProductionNode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Recipe Recipe { get; set; } = new();
    public Machine Machine { get; set; } = new();
    
    /// <summary>
    /// Number of machines required for this production step
    /// </summary>
    public double MachineCount { get; set; } = 1.0;
    
    /// <summary>
    /// Clock speed multiplier (1.0 = 100%, 2.5 = 250% max overclock)
    /// </summary>
    public double ClockSpeed { get; set; } = 1.0;
    
    /// <summary>
    /// Target production rate per minute for the primary output
    /// </summary>
    public double TargetProductionRate { get; set; }
    
    /// <summary>
    /// Actual production rate per minute (accounting for machine count and clock speed)
    /// </summary>
    public double ActualProductionRate => CalculateActualProductionRate();
    
    /// <summary>
    /// Power consumption for all machines in this node
    /// </summary>
    public double TotalPowerConsumption => Machine.PowerConsumption * MachineCount * Math.Pow(ClockSpeed, 1.6);
    
    /// <summary>
    /// Input nodes that feed into this production step
    /// </summary>
    public List<ProductionNode> InputNodes { get; set; } = new();
    
    /// <summary>
    /// Output nodes that consume from this production step
    /// </summary>
    public List<ProductionNode> OutputNodes { get; set; } = new();

    private double CalculateActualProductionRate()
    {
        if (!Recipe.Outputs.Any())
            return 0;

        var primaryOutput = Recipe.Outputs.First();
        var baseRate = (primaryOutput.Quantity * 60.0) / Recipe.ProductionTimeSeconds;
        return baseRate * MachineCount * ClockSpeed * Machine.ProductionSpeed;
    }

    public override string ToString() => $"{Recipe.Name} ({MachineCount:F1}x {Machine.Name})";
}

/// <summary>
/// Represents a complete production graph for manufacturing items
/// </summary>
public class ProductionGraph
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// All production nodes in the graph
    /// </summary>
    public List<ProductionNode> Nodes { get; set; } = new();
    
    /// <summary>
    /// Root nodes (nodes that don't have input dependencies)
    /// </summary>
    public List<ProductionNode> RootNodes => Nodes.Where(n => !n.InputNodes.Any()).ToList();
    
    /// <summary>
    /// Leaf nodes (final output nodes)
    /// </summary>
    public List<ProductionNode> LeafNodes => Nodes.Where(n => !n.OutputNodes.Any()).ToList();
    
    /// <summary>
    /// Target items and quantities to produce
    /// </summary>
    public List<ItemQuantity> TargetOutputs { get; set; } = new();
    
    /// <summary>
    /// Required raw resources and their consumption rates
    /// </summary>
    public List<ItemQuantity> RequiredResources => CalculateRequiredResources();
    
    /// <summary>
    /// Total power consumption of the entire production line
    /// </summary>
    public double TotalPowerConsumption => Nodes.Sum(n => n.TotalPowerConsumption);
    
    /// <summary>
    /// Game tier/stage this production graph is designed for
    /// </summary>
    public int GameTier { get; set; }

    private List<ItemQuantity> CalculateRequiredResources()
    {
        var resources = new Dictionary<string, double>();
        
        foreach (var rootNode in RootNodes)
        {
            foreach (var input in rootNode.Recipe.Inputs)
            {
                if (input.Item.IsRawResource)
                {
                    var requiredRate = (input.Quantity * 60.0 / rootNode.Recipe.ProductionTimeSeconds) 
                                     * rootNode.MachineCount * rootNode.ClockSpeed;
                    
                    if (resources.ContainsKey(input.Item.Id))
                        resources[input.Item.Id] += requiredRate;
                    else
                        resources[input.Item.Id] = requiredRate;
                }
            }
        }
        
        return resources.Select(kvp => 
        {
            var item = RootNodes.SelectMany(n => n.Recipe.Inputs)
                               .First(i => i.Item.Id == kvp.Key).Item;
            return new ItemQuantity(item, kvp.Value) { QuantityPerMinute = kvp.Value };
        }).ToList();
    }

    public override string ToString() => Name;
}