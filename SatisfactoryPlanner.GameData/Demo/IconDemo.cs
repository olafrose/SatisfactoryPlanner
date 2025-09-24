using SatisfactoryPlanner.GameData;
using SatisfactoryPlanner.GameData.Extensions;

namespace SatisfactoryPlanner.GameData.Demo;

class IconDemo
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🎨 Satisfactory Icon Loading Demo");
        Console.WriteLine("==================================");

        try
        {
            // Initialize the game data service
            var dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "SatisfactoryPlanner.GameData", "GameData.json");
            dataFilePath = Path.GetFullPath(dataFilePath);
            
            Console.WriteLine($"📁 Loading data from: {Path.GetDirectoryName(dataFilePath)}");
            
            var gameDataService = new GameDataService(dataFilePath);

            // Test icon service directly
            Console.WriteLine("\n🔍 Testing Icon Service:");
            
            var categories = await gameDataService.Icons.GetCategoriesAsync();
            Console.WriteLine($"Available categories: {string.Join(", ", categories)}");

            // Test loading some specific icons
            var ironIngotIconPath = await gameDataService.Icons.GetIconPathAsync("Items", "Iron Ingot");
            Console.WriteLine($"Iron Ingot icon path: {ironIngotIconPath}");
            Console.WriteLine($"Iron Ingot icon exists: {ironIngotIconPath != null && File.Exists(ironIngotIconPath)}");

            var constructorIconPath = await gameDataService.Icons.GetIconPathAsync("Buildings", "Constructor");
            Console.WriteLine($"Constructor icon path: {constructorIconPath}");
            Console.WriteLine($"Constructor icon exists: {constructorIconPath != null && File.Exists(constructorIconPath)}");

            // Test with game entities
            Console.WriteLine("\n🎯 Testing with Game Entities:");
            
            var items = await gameDataService.LoadItemsAsync();
            var machines = await gameDataService.LoadMachinesAsync();

            Console.WriteLine($"Loaded {items.Count} items and {machines.Count} machines");

            // Test a few items
            var testItems = items.Take(5).ToList();
            foreach (var item in testItems)
            {
                var hasIcon = await item.HasIconAsync(gameDataService.Icons);
                var iconPath = await item.GetIconPathAsync(gameDataService.Icons);
                Console.WriteLine($"  Item '{item.Name}': {(hasIcon ? "✅ Has icon" : "❌ No icon")} {(iconPath != null ? $"({Path.GetFileName(iconPath)})" : "")}");
            }

            // Test a few machines
            var testMachines = machines.Take(5).ToList();
            foreach (var machine in testMachines)
            {
                var hasIcon = await machine.HasIconAsync(gameDataService.Icons);
                var iconPath = await machine.GetIconPathAsync(gameDataService.Icons);
                Console.WriteLine($"  Machine '{machine.Name}': {(hasIcon ? "✅ Has icon" : "❌ No icon")} {(iconPath != null ? $"({Path.GetFileName(iconPath)})" : "")}");
            }

            // Search functionality demo
            Console.WriteLine("\n🔎 Search Demo:");
            var ironSearchResults = await gameDataService.Icons.SearchIconsAsync("Iron");
            Console.WriteLine($"Found {ironSearchResults.Count} icons containing 'Iron':");
            foreach (var result in ironSearchResults.Take(5))
            {
                Console.WriteLine($"  {result.Category}: {result.DisplayName}");
            }

            // Icon data loading demo
            Console.WriteLine("\n📊 Icon Data Loading Demo:");
            if (ironIngotIconPath != null && File.Exists(ironIngotIconPath))
            {
                var iconData = await gameDataService.Icons.GetIconDataAsync("Items", "Iron Ingot");
                Console.WriteLine($"Iron Ingot icon data: {iconData?.Length ?? 0} bytes");

                using var iconStream = await gameDataService.Icons.GetIconStreamAsync("Items", "Iron Ingot");
                Console.WriteLine($"Iron Ingot icon stream: {iconStream?.Length ?? 0} bytes");
            }

            Console.WriteLine("\n✅ Demo completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}