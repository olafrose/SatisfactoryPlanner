using System;
using Avalonia.Controls;
using Avalonia.Media;
using SatisfactoryPlanner.Avalonia.ViewModels;
using SatisfactoryPlanner.Avalonia.Controls;

namespace SatisfactoryPlanner.Avalonia.Views;

public partial class ProductionPlannerView : UserControl
{
    private ProductionFlowCanvas? _flowCanvas;

    public ProductionPlannerView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is ProductionPlannerViewModel viewModel)
        {
            InitializeFlowCanvas(viewModel);
        }
    }

    private void InitializeFlowCanvas(ProductionPlannerViewModel viewModel)
    {
        // Find the ScrollViewer that contains our graph
        var scrollViewer = this.FindControl<ScrollViewer>("ProductionGraphScrollViewer");
        if (scrollViewer == null) return;

        // Create the flow canvas
        _flowCanvas = new ProductionFlowCanvas
        {
            Background = Brushes.LightGray,
            Width = 1200,
            Height = 800
        };

        // Replace the placeholder with our canvas
        scrollViewer.Content = _flowCanvas;

        // Create nodes from the view model data
        CreateNodesFromViewModel(viewModel);
    }

    private void CreateNodesFromViewModel(ProductionPlannerViewModel viewModel)
    {
        if (_flowCanvas == null) return;

        double xOffset = 200; // Increased to ensure leftmost node is fully visible
        double yOffset = 100;

        foreach (var nodeViewModel in viewModel.ProductionNodes)
        {
            CreateNodeRecursive(nodeViewModel, xOffset, yOffset, 0, null);
        }
    }

    private void CreateNodeRecursive(ProductionNodeViewModel nodeViewModel, double baseX, double baseY, int level, ProductionNodeControl? parentNode)
    {
        if (_flowCanvas == null) return;

        var nodeControl = new ProductionNodeControl
        {
            DataContext = nodeViewModel,
            NodeName = nodeViewModel.Name,
            BuildingName = nodeViewModel.BuildingName,
            BuildingCount = nodeViewModel.BuildingCount,
            ProductionRate = nodeViewModel.ProductionRate
        };

        // Position nodes in a flow layout - inputs on left, outputs on right
        double x = baseX + (level * 220);
        double y = baseY + (level * 20); // Slight stagger for visual depth

        _flowCanvas.AddProductionNode(nodeControl, x, y);

        // Create connection from parent to this node (flow direction: parent -> child)
        if (parentNode != null)
        {
            _flowCanvas.AddConnection(parentNode, nodeControl);
        }

        // Create child nodes (dependencies)
        double childY = baseY + 50; // Start child nodes below parent
        foreach (var child in nodeViewModel.Children)
        {
            CreateNodeRecursive(child, baseX, childY, level + 1, nodeControl);
            childY += 140; // Vertical spacing between siblings
        }
    }
}