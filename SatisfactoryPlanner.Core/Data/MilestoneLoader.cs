using SatisfactoryPlanner.Core.Models;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// Loads milestones from JSON and converts them to domain models
/// </summary>
public class MilestoneLoader
{
    private readonly JsonDataLoader<GameDataDto> _jsonLoader;
    private readonly ItemLoader _itemLoader;
    private List<Milestone>? _cachedMilestones;

    public MilestoneLoader(string filePath, ItemLoader itemLoader)
    {
        _jsonLoader = new JsonDataLoader<GameDataDto>(filePath);
        _itemLoader = itemLoader;
    }

    /// <summary>
    /// Loads milestones and converts them to domain models
    /// </summary>
    public async Task<List<Milestone>> LoadMilestonesAsync()
    {
        if (_cachedMilestones != null)
            return _cachedMilestones;

        var gameData = await _jsonLoader.LoadAsync();
        var itemLookup = await _itemLoader.LoadItemsLookupAsync();
        
        _cachedMilestones = gameData.Milestones.Select(dto => ConvertToMilestone(dto, itemLookup)).ToList();
        return _cachedMilestones;
    }

    private static Milestone ConvertToMilestone(MilestoneDto dto, Dictionary<string, Item> itemLookup)
    {
        var milestone = new Milestone
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Tier = dto.Tier,
            UnlockedRecipeIds = new HashSet<string>(dto.UnlockedRecipeIds),
            UnlockedMachineIds = new HashSet<string>(dto.UnlockedMachineIds),
            IsRequired = dto.IsRequired,
            PrerequisiteMilestoneIds = new HashSet<string>(dto.PrerequisiteMilestoneIds)
        };

        // Convert cost items
        foreach (var costDto in dto.Cost)
        {
            if (itemLookup.TryGetValue(costDto.ItemId, out var item))
            {
                milestone.Cost.Add(new ItemQuantity(item, costDto.Quantity));
            }
            else
            {
                throw new InvalidOperationException($"Milestone '{dto.Id}' references unknown cost item '{costDto.ItemId}'");
            }
        }

        return milestone;
    }

    /// <summary>
    /// Clears the cached data
    /// </summary>
    public void ClearCache()
    {
        _jsonLoader.ClearCache();
        _cachedMilestones = null;
    }
}