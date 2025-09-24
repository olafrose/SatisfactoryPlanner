using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents a production node in the production graph
/// </summary>
public class ProductionNode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Recipe Recipe { get; set; } = new();
    public Building Building { get; set; } = new();
    
    /// <summary>
    /// Number of buildings required for this production step
    /// </summary>
    public double BuildingCount { get; set; } = 1.0;
    
    /// <summary>
    /// Clock speed multiplier (1.0 = 100%, 2.5 = 250% max overclock)
    /// </summary>
    public double ClockSpeed { get; set; } = 1.0;

    // Backward compatibility properties
    [Obsolete("Use Building instead of Machine to align with wiki terminology")]
    public Machine Machine 
    { 
        get => Building as Machine ?? new Machine 
        { 
            Id = Building.Id, 
            Name = Building.Name, 
            Description = Building.Description,
            Type = (MachineType)Building.Type,
            ProductionSpeed = Building.ProductionSpeed,
            PowerConsumption = Building.PowerConsumption,
            MaxInputConnections = Building.MaxInputConnections,
            MaxOutputConnections = Building.MaxOutputConnections,
            CanOverclock = Building.CanOverclock
        }; 
        set => Building = value; 
    }
    
    [Obsolete("Use BuildingCount instead of MachineCount to align with wiki terminology")]  
    public double MachineCount 
    { 
        get => BuildingCount; 
        set => BuildingCount = value; 
    }
    
    /// <summary>
    /// Target production rate per minute for the primary output
    /// </summary>
    public double TargetProductionRate { get; set; }
    
    /// <summary>
    /// Actual production rate per minute (accounting for building count and clock speed)
    /// </summary>
    public double ActualProductionRate => CalculateActualProductionRate();
    
    /// <summary>
    /// Power consumption for all buildings in this node
    /// </summary>
    public double TotalPowerConsumption => Building.PowerConsumption * BuildingCount * Math.Pow(ClockSpeed, 1.6);
    
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
        return baseRate * BuildingCount * ClockSpeed * Building.ProductionSpeed;
    }

    public override string ToString() => $"{Recipe.Name} ({BuildingCount:F1}x {Building.Name})";
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
                                     * rootNode.BuildingCount * rootNode.ClockSpeed;
                    
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