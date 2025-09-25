using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.Core;

namespace SatisfactoryPlanner.Avalonia;

public static class TestDataLoading
{
    public static async Task TestAsync()
    {
        Console.WriteLine("=== Testing Data Loading ===");
        
        try
        {
            // Test GameDataService
            var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data", "GameData");
            Console.WriteLine($"Data directory: {dataDirectory}");
            Console.WriteLine($"Data directory exists: {Directory.Exists(dataDirectory)}");
            
            if (Directory.Exists(dataDirectory))
            {
                var files = Directory.GetFiles(dataDirectory);
                Console.WriteLine($"Files in data directory: {string.Join(", ", files.Select(Path.GetFileName))}");
            }
            
            var gameDataService = new GameDataService(dataDirectory);
            
            Console.WriteLine("Loading items...");
            var items = await gameDataService.LoadItemsAsync();
            Console.WriteLine($"Loaded {items.Count()} items");
            
            if (items.Any())
            {
                var firstFew = items.Take(5);
                Console.WriteLine($"First few items: {string.Join(", ", firstFew.Select(i => i.Name))}");
            }
            
            var ironPlate = items.FirstOrDefault(i => i.Id == "iron_plate");
            Console.WriteLine($"Iron plate found: {ironPlate?.Name ?? "null"}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("=== Test Complete ===");
    }
}