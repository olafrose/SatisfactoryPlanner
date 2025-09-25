using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SatisfactoryPlanner.Avalonia.Services;

public partial class DebugService : ObservableObject
{
    private static DebugService? _instance;
    public static DebugService Instance => _instance ??= new DebugService();

    [ObservableProperty]
    private ObservableCollection<string> _debugMessages = new();

    [ObservableProperty]
    private string _debugText = string.Empty;

    public void LogDebug(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] {message}";
        
        DebugMessages.Add(logEntry);
        DebugText += logEntry + Environment.NewLine;
        
        // Keep only last 100 messages to prevent memory issues
        if (DebugMessages.Count > 100)
        {
            DebugMessages.RemoveAt(0);
        }
        
        // Also output to system debug
        System.Diagnostics.Debug.WriteLine(message);
    }

    public void ClearDebug()
    {
        DebugMessages.Clear();
        DebugText = string.Empty;
    }
}