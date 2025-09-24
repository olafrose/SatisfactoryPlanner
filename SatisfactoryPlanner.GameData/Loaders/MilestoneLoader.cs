using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Loads milestones from JSON and converts them to domain models
/// </summary>
public class MilestoneLoader
{
    private readonly JsonDataLoader<List<MilestoneDto>> _jsonLoader;
    private readonly ItemLoader _itemLoader;
    private List<Milestone>? _cachedMilestones;

    public MilestoneLoader(string gameDataDirectory, ItemLoader itemLoader)
    {
        var milestonesFilePath = Path.Combine(gameDataDirectory, "milestones.json");
        _jsonLoader = new JsonDataLoader<List<MilestoneDto>>(milestonesFilePath);
        _itemLoader = itemLoader;
    }

    /// <summary>
    /// Loads milestones and converts them to domain models
    /// </summary>
    public async Task<List<Milestone>> LoadMilestonesAsync()
    {
        if (_cachedMilestones != null)
            return _cachedMilestones;

        var milestoneDtos = await _jsonLoader.LoadAsync();
        var itemLookup = await _itemLoader.LoadItemsLookupAsync();
        
        _cachedMilestones = milestoneDtos.Select(dto => ConvertToMilestone(dto, itemLookup)).ToList();
        return _cachedMilestones;
    }

    /// <summary>
    /// Loads milestones and returns them as a lookup dictionary by ID
    /// </summary>
    public async Task<Dictionary<string, Milestone>> LoadMilestonesLookupAsync()
    {
        var milestones = await LoadMilestonesAsync();
        return milestones.ToDictionary(m => m.Id, m => m);
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