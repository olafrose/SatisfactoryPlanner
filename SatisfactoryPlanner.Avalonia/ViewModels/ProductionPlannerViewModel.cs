using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

public partial class ProductionPlannerViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProductionNodeViewModel> _productionNodes = new();

    [ObservableProperty]
    private bool _isProductSelectionPanelOpen = true;

    [ObservableProperty]
    private ObservableCollection<ProductionTargetViewModel> _productionTargets = new();
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
}

public partial class ProductionTargetViewModel : ViewModelBase
{
    [ObservableProperty]
    private Item? _targetItem;

    [ObservableProperty]
    private double _targetRate = 60.0;

    [ObservableProperty]
    private string _displayName = string.Empty;

    partial void OnTargetItemChanged(Item? value)
    {
        DisplayName = value?.Name ?? "Select Item";
    }
}