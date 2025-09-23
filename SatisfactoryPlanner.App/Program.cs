using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.App;

Console.WriteLine("=== Satisfactory Production Planner with Alternate Recipes ===");
Console.WriteLine();

// Initialize the planner service
var planner = new SatisfactoryPlannerService();

try
{
    var targetItems = new Dictionary<string, double>
    {
        ["reinforced_iron_plate"] = 10.0 // 10 per minute
    };

    Console.WriteLine("Planning production for 10 Reinforced Iron Plates per minute...");
    Console.WriteLine();

    // Plan 1: Standard recipes only
    Console.WriteLine("=== STANDARD RECIPES ONLY ===");
    var standardState = PlayerResearchState.ForTier(0);
    var standardGraph = await planner.PlanProductionAsync(targetItems, standardState);
    
    Console.WriteLine($"Production Graph: {standardGraph.Name}");
    Console.WriteLine($"Total Nodes: {standardGraph.Nodes.Count}");
    Console.WriteLine($"Total Power Consumption: {standardGraph.TotalPowerConsumption:F1} MW");
    var ironOreStandard = standardGraph.RequiredResources.FirstOrDefault(r => r.Item.Id == "iron_ore");
    Console.WriteLine($"Required Iron Ore: {ironOreStandard?.QuantityPerMinute:F1}/min");
    Console.WriteLine();

    foreach (var node in standardGraph.Nodes.OrderBy(n => n.Recipe.Name))
    {
        Console.WriteLine($"  • {node.Recipe.Name}: {node.MachineCount:F1}x {node.Machine.Name} ({(node.Recipe.IsAlternate ? "ALT" : "STD")})");
    }
    Console.WriteLine();

    // Plan 2: With efficient alternate recipes
    Console.WriteLine("=== WITH EFFICIENT ALTERNATE RECIPES ===");
    var efficientState = PlayerResearchState.EfficiencyFocused(0);
    Console.WriteLine($"Unlocked alternates: {string.Join(", ", efficientState.UnlockedAlternateRecipes)}");
    var efficientGraph = await planner.PlanProductionAsync(targetItems, efficientState);
    
    Console.WriteLine($"Production Graph: {efficientGraph.Name}");
    Console.WriteLine($"Total Nodes: {efficientGraph.Nodes.Count}");
    Console.WriteLine($"Total Power Consumption: {efficientGraph.TotalPowerConsumption:F1} MW");
    var ironOreEfficient = efficientGraph.RequiredResources.FirstOrDefault(r => r.Item.Id == "iron_ore");
    Console.WriteLine($"Required Iron Ore: {ironOreEfficient?.QuantityPerMinute:F1}/min");
    Console.WriteLine();

    foreach (var node in efficientGraph.Nodes.OrderBy(n => n.Recipe.Name))
    {
        Console.WriteLine($"  • {node.Recipe.Name}: {node.MachineCount:F1}x {node.Machine.Name} ({(node.Recipe.IsAlternate ? "ALT" : "STD")})");
    }
    Console.WriteLine();

    // Show available alternate recipes
    Console.WriteLine("=== AVAILABLE ALTERNATE RECIPES ===");
    var allRecipes = await planner.GetAvailableRecipesAsync(int.MaxValue);
    var alternateRecipes = allRecipes.Where(r => r.IsAlternate).OrderBy(r => r.Name);
    
    foreach (var recipe in alternateRecipes)
    {
        var inputs = string.Join(", ", recipe.Inputs.Select(i => $"{i.Quantity}x {i.Item.Name}"));
        var outputs = string.Join(", ", recipe.Outputs.Select(o => $"{o.Quantity}x {o.Item.Name}"));
        Console.WriteLine($"  • {recipe.Name}: {inputs} → {outputs}");
    }

    // Demonstrate milestone-based progression
    Console.WriteLine();
    Console.WriteLine("=== MILESTONE-BASED PROGRESSION ===");
    
    // Player with only basic milestones
    var basicPlayer = PlayerResearchState.WithMilestones(2, "tier0_equipment", "tier0_production");
    Console.WriteLine($"Basic Player - Completed Milestones: {string.Join(", ", basicPlayer.CompletedMilestones)}");
    
    // Player with advanced milestones
    var advancedPlayer = PlayerResearchState.WithMilestones(2, 
        "tier0_equipment", "tier0_production", "tier1_logistics", "tier2_part_assembly");
    Console.WriteLine($"Advanced Player - Completed Milestones: {string.Join(", ", advancedPlayer.CompletedMilestones)}");
    
    // Compare available content
    Console.WriteLine();
    Console.WriteLine("Recipe availability comparison:");
    var testRecipes = new[] { "iron_plate", "reinforced_iron_plate", "concrete" };
    
    foreach (var recipeId in testRecipes)
    {
        var recipe = allRecipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe != null)
        {
            var basicAvailable = basicPlayer.IsRecipeAvailable(recipe);
            var advancedAvailable = advancedPlayer.IsRecipeAvailable(recipe);
            Console.WriteLine($"  • {recipe.Name}: Basic={basicAvailable}, Advanced={advancedAvailable}");
        }
    }
    
    // Run detailed milestone demo
    await MilestoneDemo.RunMilestoneDemo();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();