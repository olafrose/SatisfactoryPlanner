using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of milestone repository that loads from JSON data
/// </summary>
public class InMemoryMilestoneRepository : IMilestoneRepository
{
    private readonly MilestoneLoader _milestoneLoader;
    private List<Milestone>? _milestones;

    public InMemoryMilestoneRepository(MilestoneLoader milestoneLoader)
    {
        _milestoneLoader = milestoneLoader;
    }

    public InMemoryMilestoneRepository(string dataFilePath, ItemLoader itemLoader)
    {
        _milestoneLoader = new MilestoneLoader(dataFilePath, itemLoader);
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_milestones == null)
        {
            _milestones = await _milestoneLoader.LoadMilestonesAsync();
        }
    }

    public async Task<List<Milestone>> GetAllMilestonesAsync()
    {
        await EnsureDataLoadedAsync();
        return _milestones!;
    }

    public async Task<Milestone?> GetMilestoneByIdAsync(string id)
    {
        await EnsureDataLoadedAsync();
        return _milestones!.FirstOrDefault(m => m.Id == id);
    }

    public async Task<List<Milestone>> GetMilestonesByTierAsync(int tier)
    {
        await EnsureDataLoadedAsync();
        return _milestones!.Where(m => m.Tier == tier).ToList();
    }

    public async Task<List<string>> GetRecipeIdsUnlockedByMilestoneAsync(string milestoneId)
    {
        var milestone = await GetMilestoneByIdAsync(milestoneId);
        return milestone?.UnlockedRecipeIds.ToList() ?? new List<string>();
    }

    public async Task<List<string>> GetMachineIdsUnlockedByMilestoneAsync(string milestoneId)
    {
        var milestone = await GetMilestoneByIdAsync(milestoneId);
        return milestone?.UnlockedMachineIds.ToList() ?? new List<string>();
    }

    public async Task<bool> AreMilestonePrerequisitesMetAsync(string milestoneId, IEnumerable<string> completedMilestones)
    {
        var milestone = await GetMilestoneByIdAsync(milestoneId);
        if (milestone == null) return false;

        var completedSet = completedMilestones.ToHashSet();
        var prerequisitesMet = milestone.PrerequisiteMilestoneIds.All(prereq => completedSet.Contains(prereq));
        
        return prerequisitesMet;
    }
}