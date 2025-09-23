namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Data transfer objects for JSON deserialization
/// </summary>
public class GameDataDto
{
    public List<ItemDto> Items { get; set; } = new();
    public List<RecipeDto> Recipes { get; set; } = new();
    public List<MilestoneDto> Milestones { get; set; } = new();
    public List<MachineDto> Machines { get; set; } = new();
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

public class MilestoneDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Tier { get; set; }
    public List<string> UnlockedRecipeIds { get; set; } = new();
    public List<string> UnlockedMachineIds { get; set; } = new();
    public bool IsRequired { get; set; } = true;
    public List<string> PrerequisiteMilestoneIds { get; set; } = new();
    public List<ItemQuantityDto> Cost { get; set; } = new();
}

public class MachineDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double ProductionSpeed { get; set; }
    public double PowerConsumption { get; set; }
    public int UnlockTier { get; set; }
    public int MaxInputConnections { get; set; }
    public int MaxOutputConnections { get; set; }
    public bool CanOverclock { get; set; } = true;
}