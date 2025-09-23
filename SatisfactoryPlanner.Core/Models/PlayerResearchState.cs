using System.Collections.Generic;
using System.Linq;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Core.Models;

/// <summary>
/// Represents the player's research progress and unlocked content
/// </summary>
public class PlayerResearchState
{
    /// <summary>
    /// Current tier the player has reached
    /// </summary>
    public int CurrentTier { get; set; } = 0;

    /// <summary>
    /// Set of milestone IDs that the player has completed
    /// </summary>
    public HashSet<string> CompletedMilestones { get; set; } = new();

    /// <summary>
    /// Set of alternate recipe IDs that the player has unlocked (from hard drives)
    /// </summary>
    public HashSet<string> UnlockedAlternateRecipes { get; set; } = new();

    /// <summary>
    /// Set of standard recipe IDs that the player has disabled (prefers alternates)
    /// </summary>
    public HashSet<string> DisabledStandardRecipes { get; set; } = new();

    /// <summary>
    /// Player's preferred optimization strategy
    /// </summary>
    public OptimizationTarget PreferredOptimization { get; set; } = OptimizationTarget.Simplicity;

    /// <summary>
    /// Whether the player allows overclocking machines
    /// </summary>
    public bool AllowOverclocking { get; set; } = true;

    /// <summary>
    /// Maximum overclocking percentage the player is comfortable with
    /// </summary>
    public double MaxOverclockPercentage { get; set; } = 250.0;

    /// <summary>
    /// Checks if a recipe is available to the player based on milestone completion
    /// </summary>
    public bool IsRecipeAvailable(Recipe recipe)
    {
        // For alternate recipes, check if unlocked
        if (recipe.IsAlternate)
            return UnlockedAlternateRecipes.Contains(recipe.Id);

        // For standard recipes, check if not disabled and milestone unlocked
        if (DisabledStandardRecipes.Contains(recipe.Id))
            return false;

        // Check if the recipe is unlocked by any completed milestone
        return IsRecipeUnlockedByMilestones(recipe.Id);
    }

    /// <summary>
    /// Checks if a recipe is unlocked by completed milestones
    /// Note: This sync version uses hardcoded milestone-recipe mappings.
    /// Use IsRecipeUnlockedByMilestonesAsync for accurate milestone checking.
    /// </summary>
    public bool IsRecipeUnlockedByMilestones(string recipeId)
    {
        // If no milestone system is used, default to tier-based unlocking
        if (!CompletedMilestones.Any())
            return true;

        // Hardcoded milestone-recipe mappings (should match GameData.json)
        var milestoneRecipes = new Dictionary<string, HashSet<string>>
        {
            ["onboarding"] = new HashSet<string> { "iron_ingot", "iron_plate", "iron_rod" },
            ["hub_upgrade_2"] = new HashSet<string> { "copper_ingot", "wire", "cable" },
            ["hub_upgrade_3"] = new HashSet<string> { "concrete", "screw", "reinforced_iron_plate" },
            ["hub_upgrade_6"] = new HashSet<string> { "biomass" },
            ["part_assembly"] = new HashSet<string> { "copper_sheet", "rotor", "modular_frame", "smart_plating" },
            ["obstacle_clearing"] = new HashSet<string> { "solid_biofuel" },
            ["basic_steel_production"] = new HashSet<string> { "steel_ingot", "steel_beam", "steel_pipe", "versatile_framework" },
            ["advanced_steel_production"] = new HashSet<string> { "encased_industrial_beam", "stator", "motor", "automated_wiring" }
        };

        // Check if any completed milestone unlocks this recipe
        foreach (var milestone in CompletedMilestones)
        {
            if (milestoneRecipes.TryGetValue(milestone, out var recipes) && recipes.Contains(recipeId))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a machine/building is available to the player
    /// </summary>
    public bool IsMachineAvailable(Machine machine)
    {
        // Check if the machine is unlocked by any completed milestone
        return IsMachineUnlockedByMilestones(machine.Id);
    }

    /// <summary>
    /// Checks if a recipe is available using milestone repository (async version)
    /// </summary>
    public async Task<bool> IsRecipeAvailableAsync(Recipe recipe, IMilestoneRepository milestoneRepository)
    {
        // For alternate recipes, check if unlocked
        if (recipe.IsAlternate)
            return UnlockedAlternateRecipes.Contains(recipe.Id);

        // For standard recipes, check if not disabled and milestone unlocked
        if (DisabledStandardRecipes.Contains(recipe.Id))
            return false;

        // Check if the recipe is unlocked by any completed milestone
        return await IsRecipeUnlockedByMilestonesAsync(recipe.Id, milestoneRepository);
    }

    /// <summary>
    /// Checks if a machine is available using milestone repository (async version)
    /// </summary>
    public async Task<bool> IsMachineAvailableAsync(Machine machine, IMilestoneRepository milestoneRepository)
    {
        // Check if the machine is unlocked by any completed milestone
        return await IsMachineUnlockedByMilestonesAsync(machine.Id, milestoneRepository);
    }

    /// <summary>
    /// Checks if a machine is unlocked by completed milestones
    /// </summary>
    public bool IsMachineUnlockedByMilestones(string machineId)
    {
        // If no milestone system is used, default to tier-based unlocking
        if (!CompletedMilestones.Any())
            return true;

        // Machine is available if unlocked by any completed milestone
        // This would need to be checked against milestone data
        // For now, we'll assume basic machines are always available in their tier
        return true;
    }

    /// <summary>
    /// Unlocks an alternate recipe
    /// </summary>
    public void UnlockAlternateRecipe(string recipeId)
    {
        UnlockedAlternateRecipes.Add(recipeId);
    }

    /// <summary>
    /// Completes a milestone and unlocks its content
    /// </summary>
    public void CompleteMilestone(string milestoneId)
    {
        CompletedMilestones.Add(milestoneId);
    }

    /// <summary>
    /// Checks if a milestone has been completed
    /// </summary>
    public bool IsMilestoneCompleted(string milestoneId)
    {
        return CompletedMilestones.Contains(milestoneId);
    }

    /// <summary>
    /// Completes multiple milestones at once
    /// </summary>
    public void CompleteMilestones(IEnumerable<string> milestoneIds)
    {
        foreach (var milestoneId in milestoneIds)
        {
            CompletedMilestones.Add(milestoneId);
        }
    }

    /// <summary>
    /// Gets all completed milestones for a specific tier
    /// </summary>
    public IEnumerable<string> GetCompletedMilestonesForTier(int tier)
    {
        // This would need milestone data to filter by tier
        // For now, return all completed milestones
        return CompletedMilestones;
    }

    /// <summary>
    /// Checks if a recipe is unlocked by completed milestones using milestone repository
    /// </summary>
    public async Task<bool> IsRecipeUnlockedByMilestonesAsync(string recipeId, IMilestoneRepository milestoneRepository)
    {
        // If no milestone system is used, default to tier-based unlocking
        if (!CompletedMilestones.Any())
            return true;

        // Check each completed milestone to see if it unlocks this recipe
        foreach (var completedMilestone in CompletedMilestones)
        {
            var unlockedRecipes = await milestoneRepository.GetRecipeIdsUnlockedByMilestoneAsync(completedMilestone);
            if (unlockedRecipes.Contains(recipeId))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a machine is unlocked by completed milestones using milestone repository
    /// </summary>
    public async Task<bool> IsMachineUnlockedByMilestonesAsync(string machineId, IMilestoneRepository milestoneRepository)
    {
        // If no milestone system is used, default to tier-based unlocking
        if (!CompletedMilestones.Any())
            return true;

        // Check each completed milestone to see if it unlocks this machine
        foreach (var completedMilestone in CompletedMilestones)
        {
            var unlockedMachines = await milestoneRepository.GetMachineIdsUnlockedByMilestoneAsync(completedMilestone);
            if (unlockedMachines.Contains(machineId))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Disables a standard recipe (useful when preferring alternates)
    /// </summary>
    public void DisableStandardRecipe(string recipeId)
    {
        DisabledStandardRecipes.Add(recipeId);
    }

    /// <summary>
    /// Creates a research state for a specific tier with basic milestones completed
    /// </summary>
    public static PlayerResearchState ForTier(int tier)
    {
        var state = new PlayerResearchState
        {
            CurrentTier = tier
        };

        // Complete basic milestones for all tiers up to current tier
        for (int t = 0; t <= tier; t++)
        {
            state.CompleteMilestones(GetBasicMilestonesForTier(t));
        }

        return state;
    }

    /// <summary>
    /// Creates a research state with specific milestones completed
    /// </summary>
    public static PlayerResearchState WithMilestones(int tier, params string[] completedMilestoneIds)
    {
        var state = new PlayerResearchState
        {
            CurrentTier = tier
        };

        state.CompleteMilestones(completedMilestoneIds);
        return state;
    }

    /// <summary>
    /// Gets the basic milestone IDs for a tier (always available)
    /// </summary>
    private static IEnumerable<string> GetBasicMilestonesForTier(int tier)
    {
        return tier switch
        {
            0 => new[] { "tier0_equipment", "tier0_production" },
            1 => new[] { "tier1_logistics", "tier1_field_research" },
            2 => new[] { "tier2_part_assembly", "tier2_obstacle_clearing" },
            3 => new[] { "tier3_coal_power", "tier3_vehicular_transport" },
            4 => new[] { "tier4_advanced_steel", "tier4_improved_melee" },
            _ => Enumerable.Empty<string>()
        };
    }

    /// <summary>
    /// Creates a research state with specific alternate recipes unlocked
    /// </summary>
    public static PlayerResearchState WithAlternates(int tier, params string[] alternateRecipeIds)
    {
        return new PlayerResearchState
        {
            CurrentTier = tier,
            UnlockedAlternateRecipes = new HashSet<string>(alternateRecipeIds)
        };
    }

    /// <summary>
    /// Creates a research state optimized for efficiency (unlocks efficient alternates and milestones)
    /// </summary>
    public static PlayerResearchState EfficiencyFocused(int tier)
    {
        var state = new PlayerResearchState
        {
            CurrentTier = tier,
            PreferredOptimization = OptimizationTarget.ResourceEfficiency,
            MaxOverclockPercentage = 200.0 // Conservative overclocking
        };

        // Complete all basic milestones for efficiency
        for (int t = 0; t <= tier; t++)
        {
            state.CompleteMilestones(GetBasicMilestonesForTier(t));
        }

        // Add efficiency-focused milestones
        if (tier >= 0)
        {
            state.CompleteMilestones(new[] { "tier0_equipment", "tier0_production" });
        }
        if (tier >= 1)
        {
            state.CompleteMilestones(new[] { "tier1_logistics", "tier1_field_research" });
        }
        if (tier >= 2)
        {
            state.CompleteMilestones(new[] { "tier2_part_assembly" });
        }

        // Add common efficiency-focused alternates
        if (tier >= 0)
        {
            state.UnlockAlternateRecipe("iron_plate_alternate_cast");
            state.UnlockAlternateRecipe("screw_alternate_cast");
            state.UnlockAlternateRecipe("wire_alternate_fused");
            state.UnlockAlternateRecipe("reinforced_iron_plate_alternate_adhered");
        }

        return state;
    }

    /// <summary>
    /// Creates a research state optimized for speed (unlocks fast alternates)
    /// </summary>
    public static PlayerResearchState SpeedFocused(int tier)
    {
        var state = new PlayerResearchState
        {
            CurrentTier = tier,
            PreferredOptimization = OptimizationTarget.Speed,
            MaxOverclockPercentage = 250.0 // Aggressive overclocking
        };

        // Add speed-focused alternates
        if (tier >= 0)
        {
            state.UnlockAlternateRecipe("wire_alternate_fused");
            state.UnlockAlternateRecipe("screw_alternate_cast");
        }

        return state;
    }
}