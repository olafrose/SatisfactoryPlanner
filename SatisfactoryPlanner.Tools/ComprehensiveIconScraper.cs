using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SatisfactoryPlanner.Tools
{
    /// <summary>
    /// Comprehensive icon scraper for all Satisfactory categories from wiki.gg
    /// </summary>
    public class ComprehensiveIconScraper
    {
        private readonly HttpClient _httpClient;
        private readonly string _outputDirectory;
        private readonly string _baseUrl = "https://satisfactory.wiki.gg";

        // Icon categories from the wiki (these actually exist and contain icons)
        private readonly Dictionary<string, (string CategoryUrl, string FolderName, int ExpectedCount)> _categories = new()
        {
            ["Items"] = ("/wiki/Category:Item_icons", "Items", 241),
            ["Buildings"] = ("/wiki/Category:Building_icons", "Buildings", 567),
            ["Fluids"] = ("/wiki/Category:Fluid_icons", "Fluids", 15),
            ["Milestones"] = ("/wiki/Category:Milestone_icons", "Milestones", 51),
            ["Vehicles"] = ("/wiki/Category:Vehicle_icons", "Vehicles", 9)
        };

        public ComprehensiveIconScraper(string? outputDirectory = null)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "SatisfactoryPlanner/1.0 (Educational Use)");
            
            // Default to GameData project folder if no output directory specified
            if (string.IsNullOrEmpty(outputDirectory))
            {
                // Navigate to GameData project folder from Tools folder
                var currentDir = Directory.GetCurrentDirectory();
                var solutionDir = Directory.GetParent(currentDir)?.FullName;
                _outputDirectory = Path.Combine(solutionDir ?? currentDir, "SatisfactoryPlanner.GameData", "Data", "Icons");
            }
            else
            {
                _outputDirectory = outputDirectory;
            }
            
            // Create output directories for all categories
            Directory.CreateDirectory(_outputDirectory);
            foreach (var category in _categories.Values)
            {
                Directory.CreateDirectory(Path.Combine(_outputDirectory, category.FolderName));
            }
            
            Console.WriteLine($"📁 Icons will be saved to: {Path.GetFullPath(_outputDirectory)}");
        }

        /// <summary>
        /// Scrapes all icons from all categories
        /// </summary>
        public async Task<Dictionary<string, Dictionary<string, string>>> ScrapeAllIconsAsync()
        {
            Console.WriteLine("Starting comprehensive icon scraping from Satisfactory wiki.gg...");
            var allIcons = new Dictionary<string, Dictionary<string, string>>();

            foreach (var category in _categories)
            {
                Console.WriteLine($"\n=== Scraping {category.Key} ===");
                var icons = await ScrapeCategoryIconsAsync(category.Key, category.Value.CategoryUrl, category.Value.FolderName);
                allIcons[category.Key] = icons;
                
                Console.WriteLine($"✓ Scraped {icons.Count} {category.Key.ToLower()} icons (expected: {category.Value.ExpectedCount})");
                
                // Extended pause between categories to avoid overwhelming the server
                await Task.Delay(10000); // Increased to 10 seconds between categories
            }

            // Save comprehensive mapping
            await SaveIconMappingAsync(allIcons);
            
            Console.WriteLine($"\n🎉 Comprehensive scraping complete! Total categories: {allIcons.Count}");
            return allIcons;
        }

        /// <summary>
        /// Scrapes icons from a specific category with pagination support and conservative rate limiting
        /// </summary>
        private async Task<Dictionary<string, string>> ScrapeCategoryIconsAsync(string categoryName, string categoryUrl, string folderName)
        {
            var iconUrls = new Dictionary<string, string>();
            var currentUrl = categoryUrl;
            var pageNumber = 1;
            var totalIconsFound = 0;
            
            try
            {
                while (!string.IsNullOrEmpty(currentUrl))
                {
                    Console.WriteLine($"📄 Processing {categoryName} page {pageNumber}...");
                    
                    // Get the current page content
                    var response = await _httpClient.GetStringAsync(_baseUrl + currentUrl);
                    
                    // Extract icon URLs using regex - looking for File: links in wiki.gg format
                    // Pattern from the category pages: [](https://satisfactory.wiki.gg/wiki/File:Iron_Ingot.png)[Iron Ingot.png]
                    var iconPattern = @"\]\(https://satisfactory\.wiki\.gg/wiki/File:([^)]+\.(png|jpg|gif))\)\[([^\]]+)\]";
                    var matches = Regex.Matches(response, iconPattern, RegexOptions.IgnoreCase);

                    if (matches.Count == 0)
                    {
                        // Fallback pattern for href links
                        iconPattern = @"href=""/wiki/File:([^""]+\.(png|jpg|gif))""[^>]*>([^<]*)</a>";
                        matches = Regex.Matches(response, iconPattern, RegexOptions.IgnoreCase);
                    }
                    
                    if (matches.Count == 0)
                    {
                        // Another fallback for simple File: links
                        iconPattern = @"/wiki/File:([^""'\s]+\.(png|jpg|gif))";
                        matches = Regex.Matches(response, iconPattern, RegexOptions.IgnoreCase);
                    }

                    var pageIconCount = 0;
                    Console.WriteLine($"Found {matches.Count} potential icons on page {pageNumber}");

                    for (int i = 0; i < matches.Count; i++)
                    {
                        var match = matches[i];
                        string fileName;
                        string displayName;
                        
                        if (match.Groups.Count >= 4 && !string.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            // Pattern with file name and display name: groups[1] = fileName, groups[3] = displayName
                            fileName = match.Groups[1].Value;
                            displayName = match.Groups[3].Value;
                        }
                        else if (match.Groups.Count >= 3 && !string.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            // Href pattern: groups[1] = fileName, groups[3] = displayName
                            fileName = match.Groups[1].Value;
                            displayName = match.Groups[3].Value.Trim();
                        }
                        else
                        {
                            // Simple file name pattern
                            fileName = match.Groups[1].Value;
                            displayName = Path.GetFileNameWithoutExtension(fileName);
                        }
                        
                        // Skip if it's not an image file
                        if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) && 
                            !fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                            !fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                            continue;

                        // Clean up the file name for existence check (same cleaning as in DownloadImageAsync)
                        var cleanFileName = Uri.UnescapeDataString(fileName);
                        cleanFileName = string.Join("_", cleanFileName.Split(Path.GetInvalidFileNameChars()));
                        var folderPath = Path.Combine(_outputDirectory, folderName);
                        var filePath = Path.Combine(folderPath, cleanFileName);
                        
                        // Skip if file already exists (before any network requests or delays)
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine($"⏭️  Skipped (already exists): {cleanFileName}");
                            iconUrls[displayName] = $"{folderName}/{cleanFileName}";
                            continue; // Skip to next iteration without delay
                        }

                        Console.WriteLine($"Processing {totalIconsFound + pageIconCount + 1} ({i+1}/{matches.Count} on page {pageNumber}): {fileName}");

                        // Get the actual image URL
                        var imageUrl = await GetImageUrlFromFilePageAsync(fileName);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            var success = await DownloadImageAsync(imageUrl, fileName, folderName);
                            if (success)
                            {
                                iconUrls[displayName] = $"{folderName}/{cleanFileName}";
                                Console.WriteLine($"✓ Downloaded: {cleanFileName}");
                                pageIconCount++;
                            }
                            
                            // Conservative delay between downloads to respect server and avoid DDoS protection
                            await Task.Delay(5000); // 5 seconds between each download
                        }
                    }

                    totalIconsFound += pageIconCount;
                    Console.WriteLine($"✅ Page {pageNumber} complete: {pageIconCount} new icons downloaded");

                    // Look for pagination - check for "next page" link
                    // Wiki.gg typically uses patterns like: href="/wiki/Category:Items?from=Iron+Ingot"
                    var nextPagePattern = @"href=""(/wiki/Category:[^""]*?\?from=[^""]+)"">next";
                    var nextPageMatch = Regex.Match(response, nextPagePattern, RegexOptions.IgnoreCase);
                    
                    if (!nextPageMatch.Success)
                    {
                        // Alternative pagination patterns
                        nextPagePattern = @"<a[^>]+href=""([^""]*)""\s*[^>]*>\s*Next\s*(?:page)?\s*(?:\d+)?\s*</a>";
                        nextPageMatch = Regex.Match(response, nextPagePattern, RegexOptions.IgnoreCase);
                    }
                    
                    if (!nextPageMatch.Success)
                    {
                        // MediaWiki standard pagination
                        nextPagePattern = @"<a[^>]+href=""([^""]*)""\s*title=""[^""]*""\s*rel=""next""";
                        nextPageMatch = Regex.Match(response, nextPagePattern, RegexOptions.IgnoreCase);
                    }

                    if (nextPageMatch.Success)
                    {
                        currentUrl = nextPageMatch.Groups[1].Value;
                        // Ensure it's a relative URL
                        if (currentUrl.StartsWith("/"))
                        {
                            // Remove the base URL part if it's already included
                            if (currentUrl.StartsWith("/wiki/"))
                            {
                                currentUrl = currentUrl.Substring(5); // Remove "/wiki" to match our base URL format
                            }
                        }
                        pageNumber++;
                        
                        Console.WriteLine($"🔄 Found next page: {currentUrl}");
                        Console.WriteLine($"📊 Total icons found so far: {totalIconsFound}");
                        
                        // Longer delay between pages to be extra respectful to the server
                        await Task.Delay(10000); // 10 seconds between pages
                    }
                    else
                    {
                        Console.WriteLine($"✅ No more pages found for {categoryName}");
                        currentUrl = null; // Exit the pagination loop
                    }
                }
                
                Console.WriteLine($"🎯 {categoryName} complete: {totalIconsFound} total icons processed across {pageNumber} pages");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error scraping {categoryName}: {ex.Message}");
            }

            return iconUrls;
        }

        /// <summary>
        /// Gets the actual image URL from a File: page with retry logic
        /// </summary>
        private async Task<string> GetImageUrlFromFilePageAsync(string fileName)
        {
            var maxRetries = 3;
            var baseDelay = 10000; // Start with 10 second delay
            
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var filePageUrl = $"{_baseUrl}/wiki/File:{fileName}";
                    var response = await _httpClient.GetStringAsync(filePageUrl);
                    
                    // Look for the direct image link with format=original
                    var imageUrlPattern = @"href=""([^""]*\.(?:png|jpg|gif)[^""]*format=original)""";
                    var match = Regex.Match(response, imageUrlPattern, RegexOptions.IgnoreCase);
                    
                    if (match.Success)
                    {
                        var imageUrl = match.Groups[1].Value;
                        // Make sure it's a full URL
                        if (imageUrl.StartsWith("//"))
                            imageUrl = "https:" + imageUrl;
                        else if (imageUrl.StartsWith("/"))
                            imageUrl = _baseUrl + imageUrl;
                            
                        return imageUrl;
                    }
                    
                    // Fallback: look for any image URL in the content
                    var fallbackPattern = @"(https://satisfactory\.wiki\.gg/images/[^""]*\.(png|jpg|gif)[^""]*)";
                    var fallbackMatch = Regex.Match(response, fallbackPattern, RegexOptions.IgnoreCase);
                    
                    if (fallbackMatch.Success)
                    {
                        return fallbackMatch.Groups[1].Value;
                    }
                    
                    break; // If we got here, the page was retrieved but no image found
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("429"))
                {
                    if (attempt < maxRetries - 1)
                    {
                        var delay = baseDelay * (int)Math.Pow(2, attempt); // Exponential backoff
                        Console.WriteLine($"⚠️  Rate limited for {fileName}, waiting {delay/1000} seconds before retry {attempt + 1}/{maxRetries}");
                        await Task.Delay(delay);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️  Max retries reached for {fileName} due to rate limiting");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️  Could not get image URL for {fileName}: {ex.Message}");
                    break;
                }
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Downloads an image to the specified folder
        /// </summary>
        private async Task<bool> DownloadImageAsync(string imageUrl, string fileName, string folderName)
        {
            try
            {
                // Clean up the image URL (decode HTML entities)
                imageUrl = imageUrl.Replace("&amp;", "&");
                
                // Clean up the file name (decode URL encoding and remove invalid characters)
                fileName = Uri.UnescapeDataString(fileName);
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
                
                // Ensure the directory exists
                var folderPath = Path.Combine(_outputDirectory, folderName);
                Directory.CreateDirectory(folderPath);
                
                var filePath = Path.Combine(folderPath, fileName);
                
                // Skip if file already exists (additional safety check)
                if (File.Exists(filePath))
                {
                    return true; // Consider existing file as successful
                }
                
                var response = await _httpClient.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(filePath, content);
                    return true;
                }
                else
                {
                    Console.WriteLine($"❌ HTTP error {response.StatusCode} for {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to download {fileName}: {ex.Message}");
            }
            
            return false;
        }

        /// <summary>
        /// Saves the comprehensive icon mapping to JSON
        /// </summary>
        private async Task SaveIconMappingAsync(Dictionary<string, Dictionary<string, string>> allIcons)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(allIcons, options);
                var mappingPath = Path.Combine(_outputDirectory, "icon_mapping.json");
                await File.WriteAllTextAsync(mappingPath, json);
                Console.WriteLine($"✓ Saved comprehensive icon mapping to: {mappingPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save icon mapping: {ex.Message}");
            }
        }

        /// <summary>
        /// Tests the scraper with a real sample from the Items category
        /// </summary>
        public async Task TestScrapingAsync()
        {
            Console.WriteLine("🧪 Testing icon scraping with real samples from Items category...");
            
            try
            {
                // Get the Items category page to find real file names
                var response = await _httpClient.GetStringAsync($"{_baseUrl}/wiki/Category:Item_icons");
                
                // Look for the first few .png files in the response
                var filePattern = @"href=""/wiki/File:([^""]+\.png)""";
                var matches = Regex.Matches(response, filePattern, RegexOptions.IgnoreCase);
                
                Console.WriteLine($"Found {matches.Count} .png files in Items category");
                
                // Test with first 3 files found
                var testCount = Math.Min(3, matches.Count);
                for (int i = 0; i < testCount; i++)
                {
                    var fileName = matches[i].Groups[1].Value;
                    Console.WriteLine($"\n--- Testing download: {fileName} ---");
                    
                    var imageUrl = await GetImageUrlFromFilePageAsync(fileName);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        Console.WriteLine($"✓ Found image URL: {imageUrl}");
                        var success = await DownloadImageAsync(imageUrl, fileName, "TestItems");
                        Console.WriteLine($"{(success ? "✓" : "❌")} Download result: {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Could not find image URL for: {fileName}");
                    }
                    
                    await Task.Delay(1000); // Longer delay for testing
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test error: {ex.Message}");
            }
            
            Console.WriteLine("\n🧪 Test scraping complete!");
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}