namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents a recipe for producing items in Satisfactory
/// </summary>
public class Recipe
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Items and quantities required as input
    /// </summary>
    public List<ItemQuantity> Inputs { get; set; } = new();
    
    /// <summary>
    /// Items and quantities produced as output
    /// </summary>
    public List<ItemQuantity> Outputs { get; set; } = new();
    
    /// <summary>
    /// Production time in seconds
    /// </summary>
    public double ProductionTimeSeconds { get; set; }
    
    /// <summary>
    /// Machines that can produce this recipe
    /// </summary>
    public List<string> CompatibleMachineIds { get; set; } = new();
    
    /// <summary>
    /// Game tier/stage when this recipe becomes available
    /// </summary>
    public int UnlockTier { get; set; }
    
    /// <summary>
    /// Whether this is an alternate recipe
    /// </summary>
    public bool IsAlternate { get; set; }

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Recipe recipe && Id == recipe.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Represents a quantity of a specific item
/// </summary>
public class ItemQuantity
{
    public Item Item { get; set; } = new();
    public double Quantity { get; set; }
    
    /// <summary>
    /// Quantity per minute (calculated from recipe production time)
    /// </summary>
    public double QuantityPerMinute { get; set; }

    public ItemQuantity() { }
    
    public ItemQuantity(Item item, double quantity)
    {
        Item = item;
        Quantity = quantity;
    }

    public override string ToString() => $"{Quantity:F2}x {Item.Name}";
}