namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Data transfer objects for JSON deserialization
/// </summary>
public class GameDataDto
{
    public List<ItemDto> Items { get; set; } = new();
    public List<RecipeDto> Recipes { get; set; } = new();
}

public class ItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsRawResource { get; set; }
    public string IconPath { get; set; } = string.Empty;
}

public class RecipeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ItemQuantityDto> Inputs { get; set; } = new();
    public List<ItemQuantityDto> Outputs { get; set; } = new();
    public double ProductionTimeSeconds { get; set; }
    public List<string> CompatibleMachineIds { get; set; } = new();
    public int UnlockTier { get; set; }
    public bool IsAlternate { get; set; }
}

public class ItemQuantityDto
{
    public string ItemId { get; set; } = string.Empty;
    public double Quantity { get; set; }
}