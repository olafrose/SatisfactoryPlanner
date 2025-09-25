using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactoryPlanner.GameData.Models;
using SatisfactoryPlanner.Core;
using SatisfactoryPlanner.Core.Models;
using SatisfactoryPlanner.Core.Services;
using SatisfactoryPlanner.Avalonia.Services;
using Avalonia.Threading;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

public partial class ProductionPlannerViewModel : ViewModelBase
{
    private readonly SatisfactoryPlannerService _plannerService;

    [ObservableProperty]
    private ObservableCollection<ProductionNodeViewModel> _productionNodes = new();

    partial void OnProductionNodesChanged(ObservableCollection<ProductionNodeViewModel> value)
    {
        DebugService.Instance.LogDebug($"ProductionNodes property changed - New collection has {value?.Count ?? 0} items");
    }

    [ObservableProperty]
    private ObservableCollection<ConnectionLineViewModel> _connectionLines = new();

    [ObservableProperty]
    private bool _isProductSelectionPanelOpen = true;

    [ObservableProperty]
    private ObservableCollection<ProductionTargetViewModel> _productionTargets = new();

    [ObservableProperty]
    private PlayerResearchState _playerState = PlayerResearchState.WithMilestones(0, "onboarding", "hub_upgrade_1", "hub_upgrade_2", "hub_upgrade_3");

    [ObservableProperty]
    private bool _isCalculating = false;

    [ObservableProperty]
    private string _calculationStatus = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Item> _availableItems = new();



    public ProductionPlannerViewModel(SatisfactoryPlannerService plannerService)
    {
        try
        {
            Console.WriteLine("ProductionPlannerViewModel constructor starting...");
            _plannerService = plannerService;
            DebugService.Instance.LogDebug("ProductionPlannerViewModel constructor called");
            DebugService.Instance.LogDebug("Starting LoadAvailableItemsAsync...");
            _ = LoadAvailableItemsAsync();
            DebugService.Instance.LogDebug("ProductionPlannerViewModel constructor completed");
        }
        catch (Exception ex)
        {
            DebugService.Instance.LogDebug($"Error in ProductionPlannerViewModel constructor: {ex.Message}");
            DebugService.Instance.LogDebug($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    // Parameterless constructor for design time
    public ProductionPlannerViewModel() : this(new SatisfactoryPlannerService())
    {
    }



    private async Task LoadAvailableItemsAsync()
    {
        System.Diagnostics.Debug.WriteLine("LoadAvailableItemsAsync called");
        Console.WriteLine("LoadAvailableItemsAsync called");
        try
        {
            System.Diagnostics.Debug.WriteLine("Loading items from planner service...");
            Console.WriteLine("Loading items from planner service...");
            var items = await _plannerService.GetAllItemsAsync();
            System.Diagnostics.Debug.WriteLine($"Loaded {items.Count()} items from planner service");
            Console.WriteLine($"Loaded {items.Count()} items from planner service");
            
            AvailableItems.Clear();
            var filteredItems = items.Where(i => !i.IsRawResource).OrderBy(i => i.Name);
            foreach (var item in filteredItems)
            {
                AvailableItems.Add(item);
            }
            System.Diagnostics.Debug.WriteLine($"Added {AvailableItems.Count} non-raw-resource items to AvailableItems collection");
            Console.WriteLine($"Added {AvailableItems.Count} non-raw-resource items to AvailableItems collection");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception in LoadAvailableItemsAsync: {ex.Message}");
            Console.WriteLine($"Exception in LoadAvailableItemsAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            // If loading fails, we'll have an empty list
        }
    }

    [RelayCommand]
    private void AddProductionTarget()
    {
        DebugService.Instance.LogDebug("AddProductionTarget called");
        var target = new ProductionTargetViewModel { Parent = this };
        ProductionTargets.Add(target);
        DebugService.Instance.LogDebug($"Added production target. Total targets: {ProductionTargets.Count}");
        
        // Don't auto-calculate for empty targets - wait for user to set item and rate
    }

    [RelayCommand]
    private void RemoveProductionTarget(ProductionTargetViewModel target)
    {
        ProductionTargets.Remove(target);
        // Recalculate production after removing a target
        _ = Task.Run(() => CalculateProductionCommand.ExecuteAsync(null));
    }

    [RelayCommand]
    private async Task CalculateProduction()
    {
        DebugService.Instance.LogDebug($"CalculateProduction called - Targets: {ProductionTargets.Count}, IsCalculating: {IsCalculating}");
        
        if (!ProductionTargets.Any() || IsCalculating)
        {
            DebugService.Instance.LogDebug("Calculation skipped - no targets or already calculating");
            return;
        }

        IsCalculating = true;
        CalculationStatus = "Calculating production chain...";
        DebugService.Instance.LogDebug("Starting production calculation");

        try
        {
            // Convert ProductionTargetViewModels to dictionary for planner service
            var targetItems = new Dictionary<string, double>();
            
            foreach (var target in ProductionTargets)
            {
                DebugService.Instance.LogDebug($"Target: {target.DisplayName}, Item: {target.TargetItem?.Name ?? "null"}, Rate: {target.TargetRate}");
                if (target.TargetItem != null)
                {
                    targetItems[target.TargetItem.Id] = target.TargetRate;
                }
            }

            DebugService.Instance.LogDebug($"Valid target items for calculation: {targetItems.Count}");
            
            if (!targetItems.Any())
            {
                CalculationStatus = "No valid production targets";
                ProductionNodes.Clear();
                DebugService.Instance.LogDebug("No valid targets - clearing nodes");
                return;
            }

            // Calculate the production graph
            DebugService.Instance.LogDebug("Calling PlanProductionAsync...");
            var productionGraph = await _plannerService.PlanProductionAsync(targetItems, PlayerState);
            DebugService.Instance.LogDebug($"Production graph calculated: {productionGraph.Nodes.Count} nodes");

            // Convert ProductionGraph nodes to ProductionNodeViewModels
            DebugService.Instance.LogDebug("Converting nodes to ViewModels...");
            await UpdateProductionNodesAsync(productionGraph);
            DebugService.Instance.LogDebug($"ProductionNodes collection now has {ProductionNodes.Count} items");

            CalculationStatus = $"Production calculated: {productionGraph.Nodes.Count} nodes, {productionGraph.TotalPowerConsumption:F1} MW";
        }
        catch (Exception ex)
        {
            DebugService.Instance.LogDebug($"Error during calculation: {ex.Message}");
            DebugService.Instance.LogDebug($"Stack trace: {ex.StackTrace}");
            CalculationStatus = $"Error: {ex.Message}";
            ProductionNodes.Clear();
        }
        finally
        {
            IsCalculating = false;
            DebugService.Instance.LogDebug("Calculation completed");
        }
    }

    private async Task UpdateProductionNodesAsync(ProductionGraph productionGraph)
    {
        DebugService.Instance.LogDebug($"UpdateProductionNodesAsync: Processing {productionGraph.Nodes.Count} nodes");
        
        // Create new collection with all nodes positioned as flow graph
        var newNodes = new ObservableCollection<ProductionNodeViewModel>();
        var orderedNodes = OrderNodesByDependencies(productionGraph.Nodes);
        var nodePositions = CalculateFlowGraphPositions(orderedNodes);
        
        for (int i = 0; i < orderedNodes.Count; i++)
        {
            var node = orderedNodes[i];
            var position = nodePositions[i];
            
            DebugService.Instance.LogDebug($"Creating ViewModel for node: {node.Recipe.Name}");
            var nodeViewModel = new ProductionNodeViewModel
            {
                Name = node.Recipe.Name,
                BuildingName = node.Building.Name,
                BuildingCount = node.BuildingCount,
                ProductionRate = node.ActualProductionRate,
                XPosition = position.X,
                YPosition = position.Y
            };

            newNodes.Add(nodeViewModel);
            DebugService.Instance.LogDebug($"Added flow node: {nodeViewModel.Name} at ({nodeViewModel.XPosition}, {nodeViewModel.YPosition})");
        }

        // Generate connection lines
        var connectionLines = GenerateConnectionLines(orderedNodes, newNodes);

        // Assign the new collections on the UI thread
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            DebugService.Instance.LogDebug($"Assigning new ProductionNodes collection with {newNodes.Count} items");
            ProductionNodes = newNodes;
            DebugService.Instance.LogDebug($"ProductionNodes property set - Count: {ProductionNodes.Count}");
            
            DebugService.Instance.LogDebug($"Assigning new ConnectionLines collection with {connectionLines.Count} items");
            ConnectionLines = connectionLines;
        });
        
        CalculationStatus += $" - {ProductionNodes.Count} production steps calculated";
    }

    private async Task ConvertChildNodesAsync(ProductionNode sourceNode, ProductionNodeViewModel parentViewModel)
    {
        foreach (var inputNode in sourceNode.InputNodes)
        {
            var childViewModel = new ProductionNodeViewModel
            {
                Name = inputNode.Recipe.Name,
                BuildingName = inputNode.Building.Name,
                BuildingCount = inputNode.BuildingCount,
                ProductionRate = inputNode.ActualProductionRate
            };

            // Recursively convert children
            await ConvertChildNodesAsync(inputNode, childViewModel);

            parentViewModel.Children.Add(childViewModel);
        }
    }

    /// <summary>
    /// Orders production nodes by their dependencies (miners first, then dependent nodes)
    /// </summary>
    private List<SatisfactoryPlanner.Core.Models.ProductionNode> OrderNodesByDependencies(
        List<SatisfactoryPlanner.Core.Models.ProductionNode> nodes)
    {
        var orderedNodes = new List<SatisfactoryPlanner.Core.Models.ProductionNode>();
        var visited = new HashSet<SatisfactoryPlanner.Core.Models.ProductionNode>();

        // First pass: Add all leaf nodes (nodes with no input dependencies, i.e., miners)
        var leafNodes = nodes.Where(n => !n.InputNodes.Any()).ToList();
        foreach (var leafNode in leafNodes)
        {
            orderedNodes.Add(leafNode);
            visited.Add(leafNode);
        }

        // Second pass: Add nodes in dependency order using topological sort
        while (visited.Count < nodes.Count)
        {
            var readyNodes = nodes
                .Where(n => !visited.Contains(n))
                .Where(n => n.InputNodes.All(inputNode => visited.Contains(inputNode)))
                .ToList();

            if (!readyNodes.Any())
            {
                // Circular dependency or other issue - just add remaining nodes
                var remainingNodes = nodes.Where(n => !visited.Contains(n)).ToList();
                orderedNodes.AddRange(remainingNodes);
                break;
            }

            foreach (var node in readyNodes.OrderBy(n => n.Recipe.Name))
            {
                orderedNodes.Add(node);
                visited.Add(node);
            }
        }

        return orderedNodes;
    }

    /// <summary>
    /// Calculates positions for nodes in a flow graph layout (left to right dependency flow)
    /// with separate chains for multiple production targets
    /// </summary>
    private List<(double X, double Y)> CalculateFlowGraphPositions(List<SatisfactoryPlanner.Core.Models.ProductionNode> orderedNodes)
    {
        var positions = new List<(double X, double Y)>();
        
        // Constants for layout
        const double nodeWidth = 180;
        const double nodeHeight = 100;
        const double horizontalSpacing = 250; // Space between columns
        const double verticalSpacing = 120;   // Space between rows
        const double chainVerticalSpacing = 200; // Space between different production chains
        const double startX = 50;
        const double startY = 50;
        
        // Group nodes by production target (separate chains)
        var productionChains = GroupNodesByProductionChain(orderedNodes);
        
        DebugService.Instance.LogDebug($"Flow graph layout: {productionChains.Count} production chains");
        
        double currentChainY = startY;
        
        // Layout each production chain separately
        foreach (var chain in productionChains)
        {
            DebugService.Instance.LogDebug($"Laying out chain for {chain.Key} with {chain.Value.Count} nodes");
            
            var chainPositions = CalculateChainPositions(chain.Value, startX, currentChainY, horizontalSpacing, verticalSpacing);
            
            // Add chain positions to overall positions list
            for (int i = 0; i < chain.Value.Count; i++)
            {
                var node = chain.Value[i];
                var nodeIndex = orderedNodes.IndexOf(node);
                if (nodeIndex >= 0 && nodeIndex < positions.Count)
                {
                    positions[nodeIndex] = chainPositions[i];
                }
                else
                {
                    // Extend positions list if needed
                    while (positions.Count <= nodeIndex)
                    {
                        positions.Add((0, 0));
                    }
                    positions[nodeIndex] = chainPositions[i];
                }
            }
            
            // Calculate the height of this chain to offset the next chain
            var chainHeight = CalculateChainHeight(chain.Value, verticalSpacing);
            currentChainY += chainHeight + chainVerticalSpacing;
        }
        
        return positions;
    }
    
    /// <summary>
    /// Groups nodes by their production target (end product)
    /// </summary>
    private List<KeyValuePair<string, List<SatisfactoryPlanner.Core.Models.ProductionNode>>> GroupNodesByProductionChain(
        List<SatisfactoryPlanner.Core.Models.ProductionNode> orderedNodes)
    {
        var chains = new Dictionary<string, List<SatisfactoryPlanner.Core.Models.ProductionNode>>();
        var processedNodes = new HashSet<SatisfactoryPlanner.Core.Models.ProductionNode>();
        
        // Find all nodes that have no output connections (final products)
        var finalNodes = orderedNodes.Where(n => !n.OutputNodes.Any()).ToList();
        
        foreach (var finalNode in finalNodes)
        {
            var chainNodes = new List<SatisfactoryPlanner.Core.Models.ProductionNode>();
            CollectChainNodes(finalNode, chainNodes, processedNodes);
            
            if (chainNodes.Any())
            {
                // Order nodes in the chain by dependency
                var orderedChainNodes = OrderNodesByDependencies(chainNodes);
                chains[finalNode.Recipe.Name] = orderedChainNodes;
            }
        }
        
        // Handle any remaining nodes that weren't part of a chain (shouldn't happen normally)
        var remainingNodes = orderedNodes.Where(n => !processedNodes.Contains(n)).ToList();
        if (remainingNodes.Any())
        {
            chains["Other"] = remainingNodes;
        }
        
        return chains.ToList();
    }
    
    /// <summary>
    /// Recursively collects all nodes in a production chain
    /// </summary>
    private void CollectChainNodes(SatisfactoryPlanner.Core.Models.ProductionNode node, 
        List<SatisfactoryPlanner.Core.Models.ProductionNode> chainNodes,
        HashSet<SatisfactoryPlanner.Core.Models.ProductionNode> processedNodes)
    {
        if (processedNodes.Contains(node) || chainNodes.Contains(node))
            return;
            
        chainNodes.Add(node);
        processedNodes.Add(node);
        
        // Recursively add all input nodes
        foreach (var inputNode in node.InputNodes)
        {
            CollectChainNodes(inputNode, chainNodes, processedNodes);
        }
    }
    
    /// <summary>
    /// Calculates positions for nodes within a single production chain
    /// </summary>
    private List<(double X, double Y)> CalculateChainPositions(
        List<SatisfactoryPlanner.Core.Models.ProductionNode> chainNodes,
        double startX, double startY, double horizontalSpacing, double verticalSpacing)
    {
        var positions = new List<(double X, double Y)>();
        var nodeToLevel = new Dictionary<SatisfactoryPlanner.Core.Models.ProductionNode, int>();
        
        // Calculate dependency levels for each node in this chain
        foreach (var node in chainNodes)
        {
            nodeToLevel[node] = CalculateNodeLevel(node, nodeToLevel);
        }
        
        // Group nodes by level within this chain
        var levelGroups = chainNodes
            .GroupBy(n => nodeToLevel[n])
            .OrderBy(g => g.Key)
            .ToList();
        
        // Position nodes within each level
        foreach (var levelGroup in levelGroups)
        {
            var level = levelGroup.Key;
            var nodesInLevel = levelGroup.ToList();
            var levelX = startX + (level * horizontalSpacing);
            
            // Center nodes vertically within the level
            var totalHeight = (nodesInLevel.Count - 1) * verticalSpacing;
            var startYForLevel = startY - (totalHeight / 2);
            
            for (int i = 0; i < nodesInLevel.Count; i++)
            {
                var node = nodesInLevel[i];
                var yPosition = startYForLevel + (i * verticalSpacing);
                
                positions.Add((levelX, yPosition));
                DebugService.Instance.LogDebug($"  Chain node {node.Recipe.Name}: ({levelX}, {yPosition})");
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// Calculates the total height needed for a production chain
    /// </summary>
    private double CalculateChainHeight(List<SatisfactoryPlanner.Core.Models.ProductionNode> chainNodes, double verticalSpacing)
    {
        var nodeToLevel = new Dictionary<SatisfactoryPlanner.Core.Models.ProductionNode, int>();
        
        foreach (var node in chainNodes)
        {
            nodeToLevel[node] = CalculateNodeLevel(node, nodeToLevel);
        }
        
        var levelGroups = chainNodes.GroupBy(n => nodeToLevel[n]);
        var maxNodesInLevel = levelGroups.Max(g => g.Count());
        
        return (maxNodesInLevel - 1) * verticalSpacing + 100; // +100 for node height
    }
    
    /// <summary>
    /// Calculates the dependency level (depth) of a node in the production chain
    /// </summary>
    private int CalculateNodeLevel(SatisfactoryPlanner.Core.Models.ProductionNode node, 
        Dictionary<SatisfactoryPlanner.Core.Models.ProductionNode, int> nodeToLevel)
    {
        // If already calculated, return cached value
        if (nodeToLevel.ContainsKey(node))
        {
            return nodeToLevel[node];
        }
        
        // Leaf nodes (miners, extractors) are at level 0
        if (!node.InputNodes.Any())
        {
            nodeToLevel[node] = 0;
            return 0;
        }
        
        // Node level is 1 + max level of its inputs
        var maxInputLevel = 0;
        foreach (var inputNode in node.InputNodes)
        {
            var inputLevel = CalculateNodeLevel(inputNode, nodeToLevel);
            maxInputLevel = Math.Max(maxInputLevel, inputLevel);
        }
        
        var level = maxInputLevel + 1;
        nodeToLevel[node] = level;
        return level;
    }

    /// <summary>
    /// Generates connection lines between production nodes to visualize item flow
    /// </summary>
    private ObservableCollection<ConnectionLineViewModel> GenerateConnectionLines(
        List<SatisfactoryPlanner.Core.Models.ProductionNode> orderedNodes,
        ObservableCollection<ProductionNodeViewModel> nodeViewModels)
    {
        var connectionLines = new ObservableCollection<ConnectionLineViewModel>();
        
        DebugService.Instance.LogDebug($"Generating connection lines for {orderedNodes.Count} nodes");
        
        for (int i = 0; i < orderedNodes.Count; i++)
        {
            var node = orderedNodes[i];
            var sourceNodeViewModel = nodeViewModels[i];
            
            // Create connections from this node to all its output nodes
            foreach (var outputNode in node.OutputNodes)
            {
                var outputIndex = orderedNodes.IndexOf(outputNode);
                if (outputIndex >= 0 && outputIndex < nodeViewModels.Count)
                {
                    var targetNodeViewModel = nodeViewModels[outputIndex];
                    
                    // Find the item being transferred (use the output item of the source node)
                    var itemName = node.Recipe.Name;
                    var flowRate = node.ActualProductionRate;
                    
                    var connectionLine = new ConnectionLineViewModel(sourceNodeViewModel, targetNodeViewModel, itemName, flowRate);
                    connectionLines.Add(connectionLine);
                    
                    DebugService.Instance.LogDebug($"Created connection: {node.Recipe.Name} -> {outputNode.Recipe.Name}");
                }
            }
        }
        
        DebugService.Instance.LogDebug($"Generated {connectionLines.Count} connection lines");
        return connectionLines;
    }
}

public partial class ProductionNodeViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _buildingName = string.Empty;

    [ObservableProperty]
    private double _buildingCount;

    [ObservableProperty]
    private double _productionRate;

    [ObservableProperty]
    private ObservableCollection<ProductionNodeViewModel> _children = new();

    [ObservableProperty]
    private bool _isExpanded = true;

    [ObservableProperty]
    private double _xPosition = 0;

    [ObservableProperty]
    private double _yPosition = 0;

    // Position change logging (can be removed once drag is stable)
    partial void OnXPositionChanged(double value)
    {
        // DebugService.Instance.LogDebug($"Node XPosition changed to {value:F1}");
    }

    partial void OnYPositionChanged(double value)
    {
        // DebugService.Instance.LogDebug($"Node YPosition changed to {value:F1}");
    }
}

public partial class ProductionTargetViewModel : ViewModelBase
{
    [ObservableProperty]
    private Item? _targetItem;

    [ObservableProperty]
    private double _targetRate = 60.0;

    [ObservableProperty]
    private string _displayName = "Select Item";

    public ProductionPlannerViewModel? Parent { get; set; }

    partial void OnTargetItemChanged(Item? value)
    {
        DisplayName = value?.Name ?? "Select Item";
        NotifyParentOfChange();
    }

    partial void OnTargetRateChanged(double value)
    {
        NotifyParentOfChange();
    }

    private void NotifyParentOfChange()
    {
        DebugService.Instance.LogDebug($"NotifyParentOfChange called - Parent: {Parent != null}, TargetItem: {TargetItem?.Name ?? "null"}");
        if (Parent != null && TargetItem != null)
        {
            DebugService.Instance.LogDebug("Triggering calculation from target change");
            _ = Task.Run(() => Parent.CalculateProductionCommand.ExecuteAsync(null));
        }
    }

    [RelayCommand]
    private void Remove()
    {
        Parent?.RemoveProductionTargetCommand.Execute(this);
    }
}