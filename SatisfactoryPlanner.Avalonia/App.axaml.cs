using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.Linq;
using Avalonia.Markup.Xaml;
using SatisfactoryPlanner.Avalonia.ViewModels;
using SatisfactoryPlanner.Avalonia.Views;

namespace SatisfactoryPlanner.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            Console.WriteLine("OnFrameworkInitializationCompleted starting...");
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("Creating desktop application...");
                
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                
                Console.WriteLine("Creating MainWindowViewModel...");
                var viewModel = new MainWindowViewModel();
                
                Console.WriteLine("Creating MainWindow...");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel,
                };
                
                Console.WriteLine("Desktop application created successfully");
            }

            base.OnFrameworkInitializationCompleted();
            Console.WriteLine("OnFrameworkInitializationCompleted completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnFrameworkInitializationCompleted: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}