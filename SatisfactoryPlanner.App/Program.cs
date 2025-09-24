using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Extensions;
using SatisfactoryPlanner.App;

Console.WriteLine("=== Satisfactory Production Planner with Alternate Recipes ===");
Console.WriteLine();

// Quick Icon Loading Test
await TestIconLoading();

Console.WriteLine("\n=== Production Planning ===");

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

    // Plan 1: Standard recipes only - Player with basic HUB upgrades that unlock reinforced iron plate
    Console.WriteLine("=== STANDARD RECIPES ONLY ===");
    var standardState = PlayerResearchState.WithMilestones(0, "onboarding", "hub_upgrade_1", "hub_upgrade_2", "hub_upgrade_3");
    var standardGraph = await planner.PlanProductionAsync(targetItems, standardState);
    
    Console.WriteLine($"Production Graph: {standardGraph.Name}");
    Console.WriteLine($"Total Nodes: {standardGraph.Nodes.Count}");
    Console.WriteLine($"Total Power Consumption: {standardGraph.TotalPowerConsumption:F1} MW");
    var ironOreStandard = standardGraph.RequiredResources.FirstOrDefault(r => r.Item.Id == "iron_ore");
    Console.WriteLine($"Required Iron Ore: {ironOreStandard?.QuantityPerMinute:F1}/min");
    Console.WriteLine();

    foreach (var node in standardGraph.Nodes.OrderBy(n => n.Recipe.Name))
    {
        Console.WriteLine($"  • {node.Recipe.Name}: {node.BuildingCount:F1}x {node.Building.Name} ({(node.Recipe.IsAlternate ? "ALT" : "STD")})");
    }
    Console.WriteLine();

    // Plan 2: With efficient alternate recipes - Player with advanced milestones
    Console.WriteLine("=== WITH EFFICIENT ALTERNATE RECIPES ===");
    var efficientState = PlayerResearchState.WithMilestones(0, "onboarding", "hub_upgrade_1", "hub_upgrade_2", "hub_upgrade_3");
    // Manually add some alternate recipes for demonstration
    efficientState.UnlockAlternateRecipe("iron_plate_alternate_cast");
    efficientState.UnlockAlternateRecipe("screw_alternate_cast");
    efficientState.UnlockAlternateRecipe("wire_alternate_fused");
    efficientState.UnlockAlternateRecipe("reinforced_iron_plate_alternate_adhered");
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
        Console.WriteLine($"  • {node.Recipe.Name}: {node.BuildingCount:F1}x {node.Building.Name} ({(node.Recipe.IsAlternate ? "ALT" : "STD")})");
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
    
    // Player with only basic milestones (completed HUB upgrades)
    var basicPlayer = PlayerResearchState.WithMilestones(0, "onboarding", "hub_upgrade_1", "hub_upgrade_2", "hub_upgrade_3");
    Console.WriteLine($"Basic Player - Completed Milestones: {string.Join(", ", basicPlayer.CompletedMilestones)}");
    
    // Player with advanced milestones (completed through Tier 2)
    var advancedPlayer = PlayerResearchState.WithMilestones(2, 
        "onboarding", "hub_upgrade_1", "hub_upgrade_2", "hub_upgrade_3", "hub_upgrade_4", "hub_upgrade_5", "hub_upgrade_6",
        "logistics_mk1", "part_assembly", "logistics_mk2", "space_elevator_phase_1");
    Console.WriteLine($"Advanced Player - Completed Milestones: {string.Join(", ", advancedPlayer.CompletedMilestones)}");
    
    // Compare available content
    Console.WriteLine();
    Console.WriteLine("Recipe availability comparison:");
    var testRecipes = new[] { "iron_plate", "reinforced_iron_plate", "concrete", "rotor", "modular_frame" };
    
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

static async Task TestIconLoading()
{
    Console.WriteLine("🎨 Testing Icon Loading...");
    
    try
    {
        // Initialize the game data service
        var gameDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "SatisfactoryPlanner.GameData", "Data", "GameData");
        gameDataDirectory = Path.GetFullPath(gameDataDirectory);
        
        if (!Directory.Exists(gameDataDirectory))
        {
            Console.WriteLine($"⚠️ GameData directory not found, skipping icon test");
            return;
        }

        var gameDataService = new GameDataService(gameDataDirectory);

        // Test icon service
        var categories = await gameDataService.Icons.GetCategoriesAsync();
        Console.WriteLine($"✅ Icon categories available: {string.Join(", ", categories)}");

        // Test specific icons
        var ironIngotIconPath = await gameDataService.Icons.GetIconPathAsync("Items", "Iron Ingot");
        Console.WriteLine($"✅ Iron Ingot icon: {(File.Exists(ironIngotIconPath ?? "") ? "Found" : "Not found")}");

        var constructorIconPath = await gameDataService.Icons.GetIconPathAsync("Buildings", "Constructor");
        Console.WriteLine($"✅ Constructor icon: {(File.Exists(constructorIconPath ?? "") ? "Found" : "Not found")}");

        Console.WriteLine("✅ Icon loading test completed!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Icon test error: {ex.Message}");
    }
}