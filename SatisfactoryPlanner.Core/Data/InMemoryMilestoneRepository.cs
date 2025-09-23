using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of milestone repository that loads from JSON data
/// </summary>
public class InMemoryMilestoneRepository : IMilestoneRepository
{
    private readonly GameDataLoader _dataLoader;
    private List<Milestone>? _milestones;

    public InMemoryMilestoneRepository(GameDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
    }

    public InMemoryMilestoneRepository(string dataFilePath)
    {
        _dataLoader = new GameDataLoader(dataFilePath);
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_milestones == null)
        {
            var (_, _, milestones, _) = await _dataLoader.LoadModelsAsync();
            _milestones = milestones;
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