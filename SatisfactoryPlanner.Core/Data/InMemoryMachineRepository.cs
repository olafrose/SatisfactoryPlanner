using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of machine repository that loads from JSON data
/// </summary>
public class InMemoryMachineRepository : IMachineRepository
{
    private readonly MachineLoader _machineLoader;
    private List<Machine>? _machines;

    public InMemoryMachineRepository(MachineLoader machineLoader)
    {
        _machineLoader = machineLoader;
    }

    public InMemoryMachineRepository(string dataFilePath)
    {
        _machineLoader = new MachineLoader(dataFilePath);
    }

    private async Task EnsureDataLoadedAsync()
    {
        if (_machines == null)
        {
            _machines = await _machineLoader.LoadMachinesAsync();
        }
    }

    public async Task<List<Machine>> GetAllMachinesAsync()
    {
        await EnsureDataLoadedAsync();
        return _machines!;
    }

    public async Task<Machine?> GetMachineByIdAsync(string id)
    {
        await EnsureDataLoadedAsync();
        return _machines!.FirstOrDefault(m => m.Id == id);
    }

    public async Task<List<Machine>> GetMachinesForRecipeAsync(string recipeId)
    {
        await EnsureDataLoadedAsync();
        // Enhanced mapping logic that handles alternate recipes
        return _machines!.Where(m => 
            // Smelter recipes (ingots)
            (recipeId.Contains("ingot") && m.Type == MachineType.Smelter) ||
            
            // Constructor recipes (basic parts)
            ((recipeId.Contains("plate") || recipeId.Contains("rod") || recipeId.Contains("screw") || 
              recipeId.Contains("wire") || recipeId.Contains("cable") || recipeId.Contains("concrete") ||
              recipeId.Contains("cast") || recipeId.Contains("fused")) && m.Type == MachineType.Constructor) ||
            
            // Assembler recipes (complex parts)
            ((recipeId.Contains("reinforced") || recipeId.Contains("rotor") || recipeId.Contains("frame") || 
              recipeId.Contains("smart_plating") || recipeId.Contains("adhered")) && m.Type == MachineType.Assembler)
        ).ToList();
    }

    public async Task<List<Machine>> GetMachinesByTypeAsync(MachineType type)
    {
        await EnsureDataLoadedAsync();
        return _machines!.Where(m => m.Type == type).ToList();
    }

    public async Task<List<Machine>> GetMachinesByTierAsync(int maxTier)
    {
        await EnsureDataLoadedAsync();
        // Since machines are now unlocked by milestones, return all machines
        // Milestone filtering should happen at a higher level
        return _machines!.ToList();
    }
}