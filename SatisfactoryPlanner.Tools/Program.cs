using SatisfactoryPlanner.Tools;

namespace SatisfactoryPlanner.Tools;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔧 Satisfactory Comprehensive Icon Scraper");
        Console.WriteLine("==========================================");
        
        // Use default GameData location (no parameter = auto-detect GameData folder)
        var scraper = new ComprehensiveIconScraper();
        
        try
        {
            if (args.Length > 0 && args[0] == "--test")
            {
                // Test mode - download a few samples
                await scraper.TestScrapingAsync();
            }
            else if (args.Length > 0 && args[0] == "--estimate")
            {
                // Estimate mode - calculate time for full scraping
                Console.WriteLine("📊 Calculating time estimate for comprehensive scraping...");
                Console.WriteLine();
                
                // Expected counts from our previous testing
                var expectedCounts = new Dictionary<string, int>
                {
                    {"Items", 241},
                    {"Buildings", 567}, 
                    {"Fluids", 15},
                    {"Milestones", 51},
                    {"Vehicles", 9}
                };
                
                var totalIcons = expectedCounts.Values.Sum();
                
                // Time calculations based on our conservative rate limiting:
                // - 5 seconds per icon download
                // - 10 seconds delay between categories
                var downloadTimeSeconds = totalIcons * 5; // 5 seconds per icon
                var categoryDelaySeconds = (expectedCounts.Count - 1) * 10; // 10 seconds between categories
                var totalTimeSeconds = downloadTimeSeconds + categoryDelaySeconds;
                
                var totalTimeMinutes = totalTimeSeconds / 60.0;
                var totalTimeHours = totalTimeMinutes / 60.0;
                
                Console.WriteLine($"Expected icon counts:");
                foreach (var category in expectedCounts)
                {
                    Console.WriteLine($"  - {category.Key}: {category.Value} icons");
                }
                Console.WriteLine($"  - Total: {totalIcons} icons");
                Console.WriteLine();
                
                Console.WriteLine($"Time breakdown with conservative rate limiting:");
                Console.WriteLine($"  - Download time: {downloadTimeSeconds:N0} seconds ({downloadTimeSeconds/60.0:F1} minutes)");
                Console.WriteLine($"  - Category delays: {categoryDelaySeconds} seconds");
                Console.WriteLine($"  - Total estimated time: {totalTimeSeconds:N0} seconds");
                Console.WriteLine($"  - That's approximately: {totalTimeMinutes:F1} minutes");
                Console.WriteLine($"  - Or about: {totalTimeHours:F1} hours");
                Console.WriteLine();
                
                if (totalTimeHours > 1)
                {
                    Console.WriteLine("⚠️  This is a long-running operation!");
                    Console.WriteLine("💡 Consider running this overnight or in the background.");
                    Console.WriteLine("🔄 The scraper includes retry logic for network issues.");
                }
                
                Console.WriteLine();
                Console.WriteLine("Note: This is a conservative estimate. Actual time may vary based on:");
                Console.WriteLine("- Network speed and latency");
                Console.WriteLine("- Server response times");
                Console.WriteLine("- Any retry attempts for failed downloads");
                Console.WriteLine("- DDoS protection triggers (would add more delays)");
                
                return;
            }
            else
            {
                // Full scraping mode
                Console.WriteLine("Starting comprehensive icon scraping from wiki.gg...");
                Console.WriteLine("This will download icons from all categories:");
                Console.WriteLine("- Items (241 expected)");
                Console.WriteLine("- Buildings (567 expected)");
                Console.WriteLine("- Fluids (15 expected)");
                Console.WriteLine("- Milestones (51 expected)");
                Console.WriteLine("- Vehicles (9 expected)");
                Console.WriteLine();
                
                var allIcons = await scraper.ScrapeAllIconsAsync();
                
                Console.WriteLine("\n📊 Scraping Summary:");
                foreach (var category in allIcons)
                {
                    Console.WriteLine($"  {category.Key}: {category.Value.Count} icons");
                }
                
                var totalIcons = allIcons.Values.Sum(dict => dict.Count);
                Console.WriteLine($"\n🎯 Total icons scraped: {totalIcons}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return;
        }
        finally
        {
            scraper.Dispose();
        }
        
        Console.WriteLine("\n✨ Icon scraping complete!");
        Console.WriteLine("Icons are saved in the 'Icons' folder with subfolders for each category.");
        Console.WriteLine("A comprehensive mapping file 'icon_mapping.json' has been created.");
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}