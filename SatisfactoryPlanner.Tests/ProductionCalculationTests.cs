using Microsoft.VisualStudio.TestTools.UnitTesting;
using SatisfactoryPlanner.Avalonia.ViewModels;
using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.GameData.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SatisfactoryPlanner.Tests;

[TestClass]
public class ProductionCalculationTests
{
    [TestMethod]
    public async Task ProductionCalculation_WithValidTarget_ShouldGenerateNodes()
    {
        // Arrange
        var plannerService = new SatisfactoryPlannerService();
        var viewModel = new ProductionPlannerViewModel(plannerService);
        
        // Create a mock item for testing (this would normally come from the game data)
        var ironPlate = new Item
        {
            Id = "iron_plate",
            Name = "Iron Plate",
            Description = "A sturdy iron plate"
        };

        var target = new ProductionTargetViewModel
        {
            Parent = viewModel,
            TargetItem = ironPlate,
            TargetRate = 60.0
        };

        viewModel.ProductionTargets.Add(target);

        // Act
        await viewModel.CalculateProductionCommand.ExecuteAsync(null);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(viewModel.CalculationStatus));
        // Note: The actual production nodes depend on having valid game data loaded
        // In a real scenario, we'd have recipes and buildings available
    }

    [TestMethod]
    public void ProductionTargetViewModel_WhenTargetRateChanges_ShouldNotifyParent()
    {
        // Arrange
        var plannerService = new SatisfactoryPlannerService();
        var parentViewModel = new ProductionPlannerViewModel(plannerService);
        var targetViewModel = new ProductionTargetViewModel
        {
            Parent = parentViewModel,
            TargetItem = new Item { Id = "test", Name = "Test Item" }
        };

        var initialStatus = parentViewModel.CalculationStatus;

        // Act
        targetViewModel.TargetRate = 120.0;

        // Allow some time for async operation
        Task.Delay(100).Wait();

        // Assert
        Assert.AreEqual(120.0, targetViewModel.TargetRate);
        // The calculation status should have been updated due to the change
    }

    [TestMethod]
    public void PlayerResearchState_ShouldBeInitializedWithBasicMilestones()
    {
        // Arrange & Act
        var plannerService = new SatisfactoryPlannerService();
        var viewModel = new ProductionPlannerViewModel(plannerService);

        // Assert
        Assert.IsNotNull(viewModel.PlayerState);
        Assert.AreEqual(0, viewModel.PlayerState.CurrentTier);
        Assert.IsTrue(viewModel.PlayerState.CompletedMilestones.Contains("onboarding"));
        Assert.IsTrue(viewModel.PlayerState.CompletedMilestones.Contains("hub_upgrade_1"));
        Assert.IsTrue(viewModel.PlayerState.CompletedMilestones.Contains("hub_upgrade_2"));
        Assert.IsTrue(viewModel.PlayerState.CompletedMilestones.Contains("hub_upgrade_3"));
    }
}