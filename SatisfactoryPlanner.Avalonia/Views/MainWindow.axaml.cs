using Avalonia.Controls;
using Avalonia.Interactivity;
using SatisfactoryPlanner.Avalonia.ViewModels;
using SatisfactoryPlanner.Avalonia.Services;

namespace SatisfactoryPlanner.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ClearDebug_Click(object? sender, RoutedEventArgs e)
    {
        DebugService.Instance.ClearDebug();
    }

    private void ToggleDebug_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.IsDebugPanelVisible = !vm.IsDebugPanelVisible;
            
            // Update button text
            if (sender is Button button)
            {
                button.Content = vm.IsDebugPanelVisible ? "Hide" : "Show";
            }
        }
    }
}