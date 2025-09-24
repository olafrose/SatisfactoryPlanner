using SatisfactoryPlanner.GameData.Models;
using SatisfactoryPlanner.GameData.Services;

namespace SatisfactoryPlanner.GameData.Extensions;

/// <summary>
/// Extension methods for easy icon access from game entities
/// </summary>
public static class IconExtensions
{
    /// <summary>
    /// Gets the icon path for an item (searches across relevant categories)
    /// </summary>
    public static async Task<string?> GetIconPathAsync(this Item item, IconService iconService)
    {
        // For now, items are primarily in "Items" category in our icon structure
        // In the future, we could search across multiple categories based on item.Categories
        var searchCategories = new[] { "Items" };
        
        // Future enhancement: map ItemCategory to icon category names
        // var searchCategories = GetIconCategoriesForItem(item);
        
        return await iconService.GetIconPathFromCategoriesAsync(searchCategories, item.Name);
    }

    /// <summary>
    /// Gets the icon data for an item as byte array (searches across relevant categories)
    /// </summary>
    public static async Task<byte[]?> GetIconDataAsync(this Item item, IconService iconService)
    {
        var searchCategories = new[] { "Items" };
        return await iconService.GetIconDataFromCategoriesAsync(searchCategories, item.Name);
    }

    /// <summary>
    /// Gets the icon data for an item as stream (searches across relevant categories)
    /// </summary>
    public static async Task<Stream?> GetIconStreamAsync(this Item item, IconService iconService)
    {
        var iconPath = await item.GetIconPathAsync(iconService);
        return iconPath != null ? await iconService.GetIconStreamAsync("Items", item.Name) : null;
    }

    /// <summary>
    /// Checks if an icon exists for an item (searches across relevant categories)
    /// </summary>
    public static async Task<bool> HasIconAsync(this Item item, IconService iconService)
    {
        var searchCategories = new[] { "Items" };
        return await iconService.IconExistsInCategoriesAsync(searchCategories, item.Name);
    }

    /// <summary>
    /// Gets the icon path for a building
    /// </summary>
    public static async Task<string?> GetIconPathAsync(this Building building, IconService iconService)
    {
        return await iconService.GetIconPathAsync("Buildings", building.Name);
    }

    /// <summary>
    /// Gets the icon path for a machine/building (obsolete - use Building overload)
    /// </summary>
    [Obsolete("Use Building overload instead of Machine overload to align with wiki terminology")]
    public static async Task<string?> GetIconPathAsync(this Machine machine, IconService iconService)
    {
        return await iconService.GetIconPathAsync("Buildings", machine.Name);
    }

    /// <summary>
    /// Gets the icon data for a building as byte array
    /// </summary>
    public static async Task<byte[]?> GetIconDataAsync(this Building building, IconService iconService)
    {
        return await iconService.GetIconDataAsync("Buildings", building.Name);
    }

    /// <summary>
    /// Gets the icon data for a machine/building as byte array (obsolete - use Building overload)
    /// </summary>
    [Obsolete("Use Building overload instead of Machine overload to align with wiki terminology")]
    public static async Task<byte[]?> GetIconDataAsync(this Machine machine, IconService iconService)
    {
        return await iconService.GetIconDataAsync("Buildings", machine.Name);
    }

    /// <summary>
    /// Gets the icon data for a building as stream
    /// </summary>
    public static async Task<Stream?> GetIconStreamAsync(this Building building, IconService iconService)
    {
        return await iconService.GetIconStreamAsync("Buildings", building.Name);
    }

    /// <summary>
    /// Gets the icon data for a machine/building as stream (obsolete - use Building overload)
    /// </summary>
    [Obsolete("Use Building overload instead of Machine overload to align with wiki terminology")]
    public static async Task<Stream?> GetIconStreamAsync(this Machine machine, IconService iconService)
    {
        return await iconService.GetIconStreamAsync("Buildings", machine.Name);
    }

    /// <summary>
    /// Checks if an icon exists for a building
    /// </summary>
    public static async Task<bool> HasIconAsync(this Building building, IconService iconService)
    {
        return await iconService.IconExistsAsync("Buildings", building.Name);
    }

    /// <summary>
    /// Checks if an icon exists for a machine/building (obsolete - use Building overload)
    /// </summary>
    [Obsolete("Use Building overload instead of Machine overload to align with wiki terminology")]
    public static async Task<bool> HasIconAsync(this Machine machine, IconService iconService)
    {
        return await iconService.IconExistsAsync("Buildings", machine.Name);
    }

    /// <summary>
    /// Gets the icon path for a milestone
    /// </summary>
    public static async Task<string?> GetIconPathAsync(this Milestone milestone, IconService iconService)
    {
        return await iconService.GetIconPathAsync("Milestones", milestone.Name);
    }

    /// <summary>
    /// Gets the icon data for a milestone as byte array
    /// </summary>
    public static async Task<byte[]?> GetIconDataAsync(this Milestone milestone, IconService iconService)
    {
        return await iconService.GetIconDataAsync("Milestones", milestone.Name);
    }

    /// <summary>
    /// Gets the icon data for a milestone as stream
    /// </summary>
    public static async Task<Stream?> GetIconStreamAsync(this Milestone milestone, IconService iconService)
    {
        return await iconService.GetIconStreamAsync("Milestones", milestone.Name);
    }

    /// <summary>
    /// Checks if an icon exists for a milestone
    /// </summary>
    public static async Task<bool> HasIconAsync(this Milestone milestone, IconService iconService)
    {
        return await iconService.IconExistsAsync("Milestones", milestone.Name);
    }
}