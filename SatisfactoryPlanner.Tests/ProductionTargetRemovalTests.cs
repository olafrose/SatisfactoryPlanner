using Microsoft.VisualStudio.TestTools.UnitTesting;
using SatisfactoryPlanner.Avalonia.ViewModels;
using System.Linq;

namespace SatisfactoryPlanner.Tests;

[TestClass]
public class ProductionTargetRemovalTests
{
    [TestMethod]
    public void AddProductionTarget_ShouldAddTargetWithParentReference()
    {
        // Arrange
        var viewModel = new ProductionPlannerViewModel();
        var initialCount = viewModel.ProductionTargets.Count;

        // Act
        viewModel.AddProductionTargetCommand.Execute(null);

        // Assert
        Assert.AreEqual(initialCount + 1, viewModel.ProductionTargets.Count);
        var addedTarget = viewModel.ProductionTargets.Last();
        Assert.IsNotNull(addedTarget.Parent);
        Assert.AreSame(viewModel, addedTarget.Parent);
    }

    [TestMethod]
    public void RemoveProductionTarget_ShouldRemoveTargetFromCollection()
    {
        // Arrange
        var viewModel = new ProductionPlannerViewModel();
        viewModel.AddProductionTargetCommand.Execute(null);
        var targetToRemove = viewModel.ProductionTargets.First();
        var initialCount = viewModel.ProductionTargets.Count;

        // Act
        viewModel.RemoveProductionTargetCommand.Execute(targetToRemove);

        // Assert
        Assert.AreEqual(initialCount - 1, viewModel.ProductionTargets.Count);
        Assert.IsFalse(viewModel.ProductionTargets.Contains(targetToRemove));
    }

    [TestMethod]
    public void ProductionTargetViewModel_RemoveCommand_ShouldCallParentRemove()
    {
        // Arrange
        var parentViewModel = new ProductionPlannerViewModel();
        parentViewModel.AddProductionTargetCommand.Execute(null);
        var target = parentViewModel.ProductionTargets.First();
        var initialCount = parentViewModel.ProductionTargets.Count;

        // Act
        target.RemoveCommand.Execute(null);

        // Assert
        Assert.AreEqual(initialCount - 1, parentViewModel.ProductionTargets.Count);
        Assert.IsFalse(parentViewModel.ProductionTargets.Contains(target));
    }

    [TestMethod]
    public void MultipleTargets_RemoveSpecificTarget_ShouldOnlyRemoveCorrectTarget()
    {
        // Arrange
        var viewModel = new ProductionPlannerViewModel();
        viewModel.AddProductionTargetCommand.Execute(null);
        viewModel.AddProductionTargetCommand.Execute(null);
        viewModel.AddProductionTargetCommand.Execute(null);
        
        var targets = viewModel.ProductionTargets.ToList();
        var targetToRemove = targets[1]; // Remove middle target

        // Act
        viewModel.RemoveProductionTargetCommand.Execute(targetToRemove);

        // Assert
        Assert.AreEqual(2, viewModel.ProductionTargets.Count);
        Assert.IsFalse(viewModel.ProductionTargets.Contains(targetToRemove));
        Assert.IsTrue(viewModel.ProductionTargets.Contains(targets[0]));
        Assert.IsTrue(viewModel.ProductionTargets.Contains(targets[2]));
    }

    [TestMethod]
    public void ProductionTargetWithoutParent_RemoveCommand_ShouldHandleGracefully()
    {
        // Arrange
        var target = new ProductionTargetViewModel(); // No parent set
        
        // Act & Assert - Should not throw exception
        target.RemoveCommand.Execute(null);
        
        // Since there's no parent, nothing should happen, but no exception should be thrown
        Assert.IsNull(target.Parent);
    }
}