using SatisfactoryPlanner.Core.Data;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.App;

public class MilestoneDemo
{
    public static async Task RunMilestoneDemo()
    {
        Console.WriteLine();
        Console.WriteLine("=== MILESTONE-BASED PROGRESSION DEMO ===");
        
        // Setup repositories
        var dataPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SatisfactoryPlanner.Core", "Data", "GameData.json");
        var itemLoader = new ItemLoader(dataPath);
        var milestoneRepo = new InMemoryMilestoneRepository(dataPath, itemLoader);
        
        // Load all milestones
        var allMilestones = await milestoneRepo.GetAllMilestonesAsync();
        
        Console.WriteLine("Available Milestones:");
        foreach (var milestone in allMilestones.OrderBy(m => m.Tier).ThenBy(m => m.Name))
        {
            var prereqs = milestone.PrerequisiteMilestoneIds.Any() 
                ? $" (requires: {string.Join(", ", milestone.PrerequisiteMilestoneIds)})"
                : "";
            var required = milestone.IsRequired ? "REQUIRED" : "OPTIONAL";
            Console.WriteLine($"  • Tier {milestone.Tier}: {milestone.Name} [{required}]{prereqs}");
            
            if (milestone.UnlockedRecipeIds.Any())
            {
                Console.WriteLine($"    Unlocks recipes: {string.Join(", ", milestone.UnlockedRecipeIds)}");
            }
            if (milestone.UnlockedMachineIds.Any())
            {
                Console.WriteLine($"    Unlocks machines: {string.Join(", ", milestone.UnlockedMachineIds)}");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("Milestone Progression Scenarios:");
        
        // Scenario 1: New player (Tier 0, basic milestones only)
        var newPlayer = new PlayerResearchState
        {
            CurrentTier = 0,
            CompletedMilestones = new HashSet<string> { "tier0_equipment" }
        };
        
        Console.WriteLine("1. New Player (completed: tier0_equipment):");
        await ShowRecipeAvailability(newPlayer, milestoneRepo, new[] { "iron_plate", "screw", "reinforced_iron_plate" });
        
        // Scenario 2: Early game player (Tier 0, all basic milestones)
        var earlyPlayer = new PlayerResearchState
        {
            CurrentTier = 0,
            CompletedMilestones = new HashSet<string> { "tier0_equipment", "tier0_production" }
        };
        
        Console.WriteLine("2. Early Player (completed: tier0_equipment, tier0_production):");
        await ShowRecipeAvailability(earlyPlayer, milestoneRepo, new[] { "iron_plate", "screw", "reinforced_iron_plate" });
        
        // Scenario 3: Mid-tier player (Tier 2, several milestones)
        var midPlayer = new PlayerResearchState
        {
            CurrentTier = 2,
            CompletedMilestones = new HashSet<string> 
            { 
                "tier0_equipment", "tier0_production", 
                "tier1_logistics", "tier2_part_assembly" 
            }
        };
        
        Console.WriteLine("3. Mid-tier Player (completed tier 0-2 required milestones):");
        await ShowRecipeAvailability(midPlayer, milestoneRepo, new[] { "iron_plate", "screw", "concrete", "reinforced_iron_plate", "rotor" });
    }
    
    private static async Task ShowRecipeAvailability(PlayerResearchState player, IMilestoneRepository milestoneRepo, string[] recipeIds)
    {
        foreach (var recipeId in recipeIds)
        {
            var isAvailable = await player.IsRecipeUnlockedByMilestonesAsync(recipeId, milestoneRepo);
            var status = isAvailable ? "✓ Available" : "✗ Locked";
            Console.WriteLine($"   {recipeId}: {status}");
        }
        Console.WriteLine();
    }
}