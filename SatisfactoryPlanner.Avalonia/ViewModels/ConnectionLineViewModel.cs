using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

/// <summary>
/// ViewModel representing a connection line between two production nodes
/// </summary>
public partial class ConnectionLineViewModel : ViewModelBase
{
    private ProductionNodeViewModel? _sourceNode;
    private ProductionNodeViewModel? _targetNode;
    private const double NodeWidth = 164; // MinWidth (160) + BorderThickness (2*2)
    private const double NodeHeight = 100;

    [ObservableProperty]
    private double _startX;

    partial void OnStartXChanged(double value) => OnPropertyChanged(nameof(PathData));

    [ObservableProperty]
    private double _startY;

    partial void OnStartYChanged(double value) => OnPropertyChanged(nameof(PathData));

    [ObservableProperty]
    private double _endX;

    partial void OnEndXChanged(double value) => OnPropertyChanged(nameof(PathData));

    [ObservableProperty]
    private double _endY;

    partial void OnEndYChanged(double value) => OnPropertyChanged(nameof(PathData));

    [ObservableProperty]
    private string _itemName = string.Empty;

    [ObservableProperty]
    private double _flowRate;

    public ConnectionLineViewModel()
    {
    }

    public ConnectionLineViewModel(ProductionNodeViewModel sourceNode, ProductionNodeViewModel targetNode, string itemName, double flowRate)
    {
        _sourceNode = sourceNode;
        _targetNode = targetNode;
        ItemName = itemName;
        FlowRate = flowRate;

        // Subscribe to position changes
        if (_sourceNode != null)
        {
            _sourceNode.PropertyChanged += OnSourceNodePropertyChanged;
        }
        if (_targetNode != null)
        {
            _targetNode.PropertyChanged += OnTargetNodePropertyChanged;
        }

        // Initialize positions
        UpdatePositions();
    }

    private void OnSourceNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductionNodeViewModel.XPosition) || 
            e.PropertyName == nameof(ProductionNodeViewModel.YPosition))
        {
            UpdateStartPosition();
        }
    }

    private void OnTargetNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductionNodeViewModel.XPosition) || 
            e.PropertyName == nameof(ProductionNodeViewModel.YPosition))
        {
            UpdateEndPosition();
        }
    }

    private void UpdatePositions()
    {
        UpdateStartPosition();
        UpdateEndPosition();
    }

    private void UpdateStartPosition()
    {
        if (_sourceNode != null)
        {
            StartX = _sourceNode.XPosition + NodeWidth; // Right edge of source node
            StartY = _sourceNode.YPosition + NodeHeight / 2;
        }
    }

    private void UpdateEndPosition()
    {
        if (_targetNode != null)
        {
            EndX = _targetNode.XPosition; // Left edge of target node  
            EndY = _targetNode.YPosition + NodeHeight / 2;
        }
    }

    /// <summary>
    /// Gets the path data for drawing an arrow line
    /// </summary>
    public string PathData => CreateArrowPath();

    private string CreateArrowPath()
    {
        // Simple straight line from start to end
        return $"M {StartX},{StartY} L {EndX},{EndY}";
    }
}