namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents an item that can be produced, consumed, or extracted in Satisfactory
/// </summary>
public class Item
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemCategory Category { get; set; }
    public bool IsRawResource { get; set; } // For mining/extraction
    public string IconPath { get; set; } = string.Empty;

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Item item && Id == item.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Categories of items in Satisfactory
/// </summary>
public enum ItemCategory
{
    RawResource,      // Iron Ore, Copper Ore, etc.
    Ingot,           // Iron Ingot, Copper Ingot, etc.
    BasicPart,       // Iron Plate, Iron Rod, etc.
    IntermediatePart, // Reinforced Iron Plate, Rotor, etc.
    AdvancedPart,    // Motor, Computer, etc.
    Fuel,            // Coal, Oil, etc.
    Liquid,          // Water, Oil, Fuel, etc.
    SpaceElevator,   // Space Elevator parts
    MAM,             // MAM research parts
    Equipment,       // Tools and equipment
    Building         // Building components
}