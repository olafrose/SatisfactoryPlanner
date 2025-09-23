using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Core.Services;

namespace SatisfactoryPlanner.Tests;

[TestClass]
public sealed class ProductionPlannerTests
{
    private SatisfactoryPlannerService _planner = null!;

    [TestInitialize]
    public void Setup()
    {
        _planner = new SatisfactoryPlannerService();
    }

    [TestMethod]
    public async Task CanPlanBasicIronIngotProduction()
    {
        // Arrange
        var targetItems = new Dictionary<string, double>
        {
            ["iron_ingot"] = 30.0 // 30 iron ingots per minute
        };

        // Act
        var graph = await _planner.PlanProductionAsync(targetItems, gameTier: 0);

        // Assert
        Assert.IsNotNull(graph);
        Assert.AreEqual(1, graph.Nodes.Count);
        Assert.AreEqual("iron_ingot", graph.Nodes.First().Recipe.Id);
        Assert.IsTrue(graph.RequiredResources.Any(r => r.Item.Id == "iron_ore"));
    }

    [TestMethod]
    public async Task CanPlanComplexReinforcedIronPlateProduction()
    {
        // Arrange
        var targetItems = new Dictionary<string, double>
        {
            ["reinforced_iron_plate"] = 5.0 // 5 reinforced iron plates per minute
        };

        // Act
        var graph = await _planner.PlanProductionAsync(targetItems, gameTier: 0);

        // Assert
        Assert.IsNotNull(graph);
        Assert.IsTrue(graph.Nodes.Count > 1); // Should have multiple production steps
        Assert.IsTrue(graph.RequiredResources.Any(r => r.Item.Id == "iron_ore"));
        Assert.IsTrue(graph.TotalPowerConsumption > 0);
    }

    [TestMethod]
    public async Task ProductionAnalysisReturnsValidResults()
    {
        // Arrange
        var targetItems = new Dictionary<string, double>
        {
            ["iron_plate"] = 20.0
        };
        var graph = await _planner.PlanProductionAsync(targetItems, gameTier: 0);

        // Act
        var analysis = _planner.AnalyzeProduction(graph);

        // Assert
        Assert.IsNotNull(analysis);
        Assert.IsTrue(analysis.TotalMachines > 0);
        Assert.IsTrue(analysis.TotalPowerConsumption > 0);
        Assert.IsNotNull(analysis.RequiredResources);
        Assert.IsTrue(analysis.EfficiencyScore >= 0);
    }

    [TestMethod]
    public async Task CanGetAvailableItemsAndMachines()
    {
        // Act
        var items = await _planner.GetAllItemsAsync();
        var machines = await _planner.GetAvailableMachinesAsync(0);
        var recipes = await _planner.GetAvailableRecipesAsync(0);

        // Assert
        Assert.IsTrue(items.Count > 0);
        Assert.IsTrue(machines.Count > 0);
        Assert.IsTrue(recipes.Count > 0);
        
        // Check that we have basic items
        Assert.IsTrue(items.Any(i => i.Id == "iron_ore"));
        Assert.IsTrue(items.Any(i => i.Id == "iron_ingot"));
        Assert.IsTrue(items.Any(i => i.Id == "reinforced_iron_plate"));
    }
}
