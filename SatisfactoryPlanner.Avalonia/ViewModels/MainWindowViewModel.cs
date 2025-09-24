using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private MilestoneManagerViewModel _milestoneManager;

    [ObservableProperty]
    private ProductionPlannerViewModel _productionPlanner;

    public string Title { get; } = "Satisfactory Production Planner";

    public MainWindowViewModel()
    {
        _milestoneManager = new MilestoneManagerViewModel();
        _productionPlanner = new ProductionPlannerViewModel();
        
        InitializeTestData();
    }

    private void InitializeTestData()
    {
        // Create some test milestone tiers
        var hubTier = new MilestoneTierViewModel
        {
            Name = "HUB Upgrade",
            TierNumber = 0,
            IsSelected = true
        };

        // Add some test milestones
        hubTier.Milestones.Add(new MilestoneItemViewModel
        {
            Name = "HUB Upgrade 1",
            Description = "Unlock basic buildings",
            IsCompleted = true
        });

        hubTier.Milestones.Add(new MilestoneItemViewModel
        {
            Name = "HUB Upgrade 2", 
            Description = "Unlock smelting",
            IsCompleted = true
        });

        hubTier.Milestones.Add(new MilestoneItemViewModel
        {
            Name = "HUB Upgrade 3",
            Description = "Unlock logistics",
            IsCompleted = false
        });

        var tier1 = new MilestoneTierViewModel
        {
            Name = "Tier 1",
            TierNumber = 1
        };

        tier1.Milestones.Add(new MilestoneItemViewModel
        {
            Name = "Base Building",
            Description = "Basic infrastructure",
            IsCompleted = false
        });

        tier1.Milestones.Add(new MilestoneItemViewModel
        {
            Name = "Logistics Mk.1",
            Description = "Belt logistics",
            IsCompleted = false
        });

        MilestoneManager.Tiers.Add(hubTier);
        MilestoneManager.Tiers.Add(tier1);
        MilestoneManager.SelectedTier = hubTier;

        // Add some test production data
        var ironPlateNode = new ProductionNodeViewModel
        {
            Name = "Iron Plate",
            BuildingName = "Constructor",
            BuildingCount = 2.0,
            ProductionRate = 60.0
        };

        var ironIngotNode = new ProductionNodeViewModel
        {
            Name = "Iron Ingot",
            BuildingName = "Smelter", 
            BuildingCount = 2.0,
            ProductionRate = 60.0
        };

        ironPlateNode.Children.Add(ironIngotNode);
        ProductionPlanner.ProductionNodes.Add(ironPlateNode);

        // Add a production target
        ProductionPlanner.ProductionTargets.Add(new ProductionTargetViewModel
        {
            DisplayName = "Iron Plate",
            TargetRate = 60.0
        });
    }
}
