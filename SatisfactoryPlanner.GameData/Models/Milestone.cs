namespace SatisfactoryPlanner.GameData.Models;

/// <summary>
/// Represents a milestone within a tier that unlocks specific content
/// </summary>
public class Milestone
{
    /// <summary>
    /// Unique identifier for the milestone
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the milestone
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this milestone provides
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The tier this milestone belongs to
    /// </summary>
    public int Tier { get; set; }

    /// <summary>
    /// Recipe IDs unlocked by completing this milestone
    /// </summary>
    public HashSet<string> UnlockedRecipeIds { get; set; } = new();

    /// <summary>
    /// Machine/building IDs unlocked by completing this milestone
    /// </summary>
    public HashSet<string> UnlockedMachineIds { get; set; } = new();

    /// <summary>
    /// Item IDs unlocked by completing this milestone
    /// </summary>
    public HashSet<string> UnlockedItemIds { get; set; } = new();

    /// <summary>
    /// Whether this milestone is required to progress to the next tier
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Prerequisites - other milestone IDs that must be completed first
    /// </summary>
    public HashSet<string> PrerequisiteMilestoneIds { get; set; } = new();

    /// <summary>
    /// Cost to unlock this milestone (items required)
    /// </summary>
    public List<ItemQuantity> Cost { get; set; } = new();

    public override string ToString() => Name;
    public override bool Equals(object? obj) => obj is Milestone milestone && Id == milestone.Id;
    public override int GetHashCode() => Id.GetHashCode();
}