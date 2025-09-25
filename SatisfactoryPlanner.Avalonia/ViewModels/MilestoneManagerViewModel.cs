using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SatisfactoryPlanner.GameData.Models;

namespace SatisfactoryPlanner.Avalonia.ViewModels;

public partial class MilestoneManagerViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<MilestoneTierViewModel> _tiers = new();

    [ObservableProperty]
    private bool _isPanelExpanded = false;

    [ObservableProperty]
    private MilestoneTierViewModel? _selectedTier;

    [RelayCommand]
    private void TogglePanel()
    {
        IsPanelExpanded = !IsPanelExpanded;
    }

    [RelayCommand]
    private void SelectTier(MilestoneTierViewModel tier)
    {
        SelectedTier = tier;
    }
}

public partial class MilestoneTierViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _tierNumber;

    [ObservableProperty]
    private ObservableCollection<MilestoneItemViewModel> _milestones = new();

    [ObservableProperty]
    private bool _isSelected;
}

public partial class MilestoneItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private Milestone? _milestone;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string? _iconPath;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _milestoneId = string.Empty;

    [ObservableProperty]
    private bool _isRequired = true;

    partial void OnMilestoneChanged(Milestone? value)
    {
        if (value != null)
        {
            Name = value.Name;
            Description = value.Description;
            // Icon path will be set by the icon service
        }
    }

    [RelayCommand]
    private void ToggleCompletion()
    {
        IsCompleted = !IsCompleted;
    }
}