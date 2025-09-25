using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Models;
using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Avalonia.Services;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private MilestoneManagerViewModel _milestoneManager;

    [ObservableProperty]
    private ProductionPlannerViewModel _productionPlanner;

    [ObservableProperty]
    private bool _isDebugPanelVisible = false; // Start hidden by default

    public DebugService Debug => DebugService.Instance;

    private readonly GameDataService _gameDataService;
    private readonly SatisfactoryPlannerService _plannerService;
    private bool _isInitialized = false;

    public string Title { get; } = "Satisfactory Production Planner";

    public MainWindowViewModel()
    {
        try
        {
            Debug.LogDebug("MainWindowViewModel constructor starting...");
            
            // Initialize GameData service
            var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data", "GameData");
            Debug.LogDebug($"Data directory: {dataDirectory}");
            _gameDataService = new GameDataService(dataDirectory);
            
            // Initialize planner service
            Debug.LogDebug("Initializing planner service...");
            _plannerService = new SatisfactoryPlannerService();
            
            Debug.LogDebug("Creating milestone manager...");
            _milestoneManager = new MilestoneManagerViewModel();
            
            Debug.LogDebug("Creating production planner...");
            _productionPlanner = new ProductionPlannerViewModel(_plannerService);
            
            Debug.LogDebug("MainWindowViewModel constructor completed - starting async initialization");
            
            // Initialize milestone data
            _ = InitializeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in MainWindowViewModel constructor: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task InitializeAsync()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        try
        {
            await LoadMilestoneData();
        }
        catch (Exception ex)
        {
            // If loading real data fails, fall back to test data
            Console.WriteLine($"Failed to load milestone data: {ex.Message}");
            InitializeTestData();
        }
    }

    private async Task LoadMilestoneData()
    {
        var milestones = await _gameDataService.LoadMilestonesAsync();
        
        // Group milestones by tier
        var milestonesByTier = milestones
            .GroupBy(m => m.Tier)
            .OrderBy(g => g.Key)
            .ToList();

        MilestoneManager.Tiers.Clear();

        foreach (var tierGroup in milestonesByTier)
        {
            var tierViewModel = new MilestoneTierViewModel
            {
                Name = GetTierName(tierGroup.Key),
                TierNumber = tierGroup.Key,
                IsSelected = tierGroup.Key == 0 // Select HUB tier by default
            };

            foreach (var milestone in tierGroup.OrderBy(m => m.Name))
            {
                tierViewModel.Milestones.Add(new MilestoneItemViewModel
                {
                    Name = milestone.Name,
                    Description = milestone.Description,
                    IsCompleted = false, // User can toggle these
                    MilestoneId = milestone.Id,
                    IsRequired = milestone.IsRequired
                });
            }
            
            MilestoneManager.Tiers.Add(tierViewModel);
        }

        // Select the first tier if any exist
        if (MilestoneManager.Tiers.Count > 0)
        {
            MilestoneManager.SelectedTier = MilestoneManager.Tiers.First();
        }
    }

    private static string GetTierName(int tier)
    {
        return tier switch
        {
            0 => "HUB Upgrade",
            1 => "Tier 1 - Base Building",
            2 => "Tier 2 - Resource Processing", 
            3 => "Tier 3 - Basic Steel Production",
            4 => "Tier 4 - Advanced Steel Production",
            5 => "Tier 5 - Oil Processing",
            6 => "Tier 6 - Industrial Manufacturing",
            7 => "Tier 7 - Bauxite Refinement",
            8 => "Tier 8 - Advanced Refinement",
            _ => $"Tier {tier}"
        };
    }

    private void InitializeTestData()
    {
        // Fallback test data if real data loading fails
        MilestoneManager.Tiers.Clear();
        
        var hubTier = new MilestoneTierViewModel
        {
            Name = "HUB Upgrade",
            TierNumber = 0,
            IsSelected = true
        };

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
    }
    
    private async Task InitializeTestProductionData()
    {
        // Clear any existing targets first
        ProductionPlanner.ProductionTargets.Clear();
        
        // Add a production target with actual item from game data
        await AddInitialProductionTargetAsync();
    }

    private async Task AddInitialProductionTargetAsync()
    {
        System.Diagnostics.Debug.WriteLine("AddInitialProductionTargetAsync called");
        Console.WriteLine("AddInitialProductionTargetAsync called");
        
        // Check if there are already production targets to avoid duplicates
        if (ProductionPlanner.ProductionTargets.Any())
        {
            System.Diagnostics.Debug.WriteLine("Production targets already exist, skipping initialization");
            return;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine("Loading items from game data...");
            // Get Iron Plate item from game data
            var items = await _gameDataService.LoadItemsAsync();
            System.Diagnostics.Debug.WriteLine($"Loaded {items.Count()} items from game data");
            
            var ironPlateItem = items.FirstOrDefault(i => i.Id == "iron_plate");
            System.Diagnostics.Debug.WriteLine($"Iron plate item found: {ironPlateItem?.Name ?? "null"}");

            if (ironPlateItem != null)
            {
                var target = new ProductionTargetViewModel
                {
                    Parent = ProductionPlanner,
                    TargetRate = 60.0
                };
                
                // Set TargetItem after construction to trigger the property change
                target.TargetItem = ironPlateItem;
                System.Diagnostics.Debug.WriteLine($"Created target with item: {target.DisplayName}");

                ProductionPlanner.ProductionTargets.Add(target);
                System.Diagnostics.Debug.WriteLine($"Added target to collection. Total targets: {ProductionPlanner.ProductionTargets.Count}");
                
                // Trigger automatic calculation
                System.Diagnostics.Debug.WriteLine("Triggering calculation...");
                _ = Task.Run(async () => await ProductionPlanner.CalculateProductionCommand.ExecuteAsync(null));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Iron plate item not found in game data");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in AddInitialProductionTargetAsync: {ex.Message}");
            // Fallback to basic target without item if game data fails to load
            var fallbackTarget = new ProductionTargetViewModel
            {
                Parent = ProductionPlanner,
                DisplayName = "Iron Plate (fallback)",
                TargetRate = 60.0
            };
            ProductionPlanner.ProductionTargets.Add(fallbackTarget);
            System.Diagnostics.Debug.WriteLine("Added fallback target");
        }
    }
}
