using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Data;

/// <summary>
/// In-memory implementation of machine repository with Satisfactory game data
/// </summary>
public class InMemoryMachineRepository : IMachineRepository
{
    private readonly List<Machine> _machines;

    public InMemoryMachineRepository()
    {
        _machines = InitializeMachines();
    }

    public Task<List<Machine>> GetAllMachinesAsync()
    {
        return Task.FromResult(_machines);
    }

    public Task<Machine?> GetMachineByIdAsync(string id)
    {
        return Task.FromResult(_machines.FirstOrDefault(m => m.Id == id));
    }

    public Task<List<Machine>> GetMachinesForRecipeAsync(string recipeId)
    {
        // This would typically check which machines are compatible with the recipe
        // For now, return machines based on simple mapping
        return Task.FromResult(_machines.Where(m => 
            (recipeId.Contains("ingot") && m.Type == MachineType.Smelter) ||
            (recipeId.Contains("plate") || recipeId.Contains("rod") || recipeId.Contains("screw") || recipeId.Contains("wire") || recipeId.Contains("cable")) && m.Type == MachineType.Constructor ||
            (recipeId.Contains("reinforced")) && m.Type == MachineType.Assembler
        ).ToList());
    }

    public Task<List<Machine>> GetMachinesByTypeAsync(MachineType type)
    {
        return Task.FromResult(_machines.Where(m => m.Type == type).ToList());
    }

    public Task<List<Machine>> GetMachinesByTierAsync(int maxTier)
    {
        return Task.FromResult(_machines.Where(m => m.UnlockTier <= maxTier).ToList());
    }

    private List<Machine> InitializeMachines()
    {
        return new List<Machine>
        {
            new Machine
            {
                Id = "smelter",
                Name = "Smelter",
                Description = "Smelts ore into ingots",
                Type = MachineType.Smelter,
                ProductionSpeed = 1.0,
                PowerConsumption = 4.0,
                UnlockTier = 0,
                MaxInputConnections = 1,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "foundry",
                Name = "Foundry",
                Description = "Advanced smelting with multiple inputs",
                Type = MachineType.Smelter,
                ProductionSpeed = 1.0,
                PowerConsumption = 16.0,
                UnlockTier = 1,
                MaxInputConnections = 2,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "constructor",
                Name = "Constructor",
                Description = "Constructs basic parts from single inputs",
                Type = MachineType.Constructor,
                ProductionSpeed = 1.0,
                PowerConsumption = 4.0,
                UnlockTier = 0,
                MaxInputConnections = 1,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "assembler",
                Name = "Assembler",
                Description = "Assembles parts from multiple inputs",
                Type = MachineType.Assembler,
                ProductionSpeed = 1.0,
                PowerConsumption = 15.0,
                UnlockTier = 0,
                MaxInputConnections = 2,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "manufacturer",
                Name = "Manufacturer",
                Description = "Manufactures complex parts from multiple inputs",
                Type = MachineType.Manufacturer,
                ProductionSpeed = 1.0,
                PowerConsumption = 55.0,
                UnlockTier = 2,
                MaxInputConnections = 4,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "refinery",
                Name = "Refinery",
                Description = "Refines liquids and gases",
                Type = MachineType.Refinery,
                ProductionSpeed = 1.0,
                PowerConsumption = 30.0,
                UnlockTier = 3,
                MaxInputConnections = 3,
                MaxOutputConnections = 2,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "miner_mk1",
                Name = "Miner Mk.1",
                Description = "Extracts solid resources from resource nodes",
                Type = MachineType.Extractor,
                ProductionSpeed = 1.0,
                PowerConsumption = 5.0,
                UnlockTier = 0,
                MaxInputConnections = 0,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "miner_mk2",
                Name = "Miner Mk.2",
                Description = "Faster extraction of solid resources",
                Type = MachineType.Extractor,
                ProductionSpeed = 2.0,
                PowerConsumption = 12.0,
                UnlockTier = 2,
                MaxInputConnections = 0,
                MaxOutputConnections = 1,
                CanOverclock = true
            },
            
            new Machine
            {
                Id = "miner_mk3",
                Name = "Miner Mk.3",
                Description = "Fastest extraction of solid resources",
                Type = MachineType.Extractor,
                ProductionSpeed = 4.0,
                PowerConsumption = 30.0,
                UnlockTier = 4,
                MaxInputConnections = 0,
                MaxOutputConnections = 1,
                CanOverclock = true
            }
        };
    }
}