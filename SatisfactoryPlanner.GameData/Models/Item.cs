namespace SatisfactoryPlanner.GameData.Models;

/// <summary>
/// Represents an item that can be produced, consumed, or extracted in Satisfactory
/// </summary>
public class Item
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Primary category for the item (for backwards compatibility and primary classification)
    /// </summary>
    public ItemCategory PrimaryCategory { get; set; }
    
    /// <summary>
    /// Backward compatibility property for existing code that expects a single Category
    /// </summary>
    public ItemCategory Category 
    { 
        get => PrimaryCategory; 
        set 
        { 
            PrimaryCategory = value; 
            Categories.Clear();
            Categories.Add(value);
        } 
    }
    
    /// <summary>
    /// All categories this item belongs to (many-to-many relationship)
    /// An item can belong to multiple categories (e.g., Uranium Fuel Rod is both Fuel and Radioactive)
    /// </summary>
    public HashSet<ItemCategory> Categories { get; set; } = new();
    
    public bool IsRawResource { get; set; } // For mining/extraction
    public string IconPath { get; set; } = string.Empty;

    /// <summary>
    /// Checks if this item belongs to a specific category
    /// </summary>
    public bool HasCategory(ItemCategory category) => Categories.Contains(category);
    
    /// <summary>
    /// Adds a category to this item
    /// </summary>
    public void AddCategory(ItemCategory category) => Categories.Add(category);

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Item item && Id == item.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Categories of items in Satisfactory (based on official wiki organization)
/// Items can belong to multiple categories (many-to-many relationship)
/// </summary>
public enum ItemCategory
{
    // Official nine item categories from wiki
    Biomass,             // Wood, leaves, mycelia, alien remains, etc.
    BuildingMaterials,   // Concrete, steel beams, encased beams, etc.
    CraftingComponents,  // Iron rods, screws, wires, plates, rotors, etc.
    Equipment,           // Chainsaws, weapons, tools, jetpack, etc.
    EquipmentMaterials,  // Ammo, fuel for equipment, gas filters, etc.
    Fuels,              // Coal, petroleum coke, biofuel, fuel rods, etc.
    Ores,               // Iron ore, copper ore, limestone, raw quartz, etc.
    ProjectAssemblyParts, // Space elevator components, assembly parts, etc.
    Radioactive,        // Uranium, plutonium, nuclear waste, etc.
    
    // Additional technical categories for processing
    Fluids,             // Liquids and gases
    Buildings,          // Building/machine components
    Milestones,         // Milestone-related items
    Vehicles,           // Vehicle parts and components
    Special,            // Hard drives, power shards, etc.
    
    // Legacy/compatibility
    Other               // Miscellaneous or uncategorized items
}