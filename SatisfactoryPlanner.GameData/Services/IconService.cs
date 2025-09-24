using System.Text.Json;

namespace SatisfactoryPlanner.GameData.Services;

/// <summary>
/// Service for loading and managing Satisfactory icons
/// </summary>
public class IconService
{
    private readonly string _iconsPath;
    private readonly string _mappingPath;
    private Dictionary<string, Dictionary<string, string>>? _iconMapping;

    public IconService(string gameDataPath)
    {
        // gameDataPath is the directory containing GameData.json (e.g., Data/GameData)
        // Icons are in the Data folder, so we go up one level from GameData folder
        var dataFolder = Path.GetDirectoryName(gameDataPath) ?? throw new ArgumentException("Invalid game data path", nameof(gameDataPath));
        _iconsPath = Path.Combine(dataFolder, "Icons");
        _mappingPath = Path.Combine(_iconsPath, "icon_mapping.json");
    }

    /// <summary>
    /// Loads the icon mapping from the JSON file
    /// </summary>
    public async Task<Dictionary<string, Dictionary<string, string>>> LoadIconMappingAsync()
    {
        if (_iconMapping != null)
            return _iconMapping;

        if (!File.Exists(_mappingPath))
            throw new FileNotFoundException($"Icon mapping file not found: {_mappingPath}");

        var json = await File.ReadAllTextAsync(_mappingPath);
        _iconMapping = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) 
                      ?? throw new InvalidOperationException("Failed to deserialize icon mapping");

        return _iconMapping;
    }

    /// <summary>
    /// Gets the file path for an icon by category and display name
    /// </summary>
    public async Task<string?> GetIconPathAsync(string category, string displayName)
    {
        var mapping = await LoadIconMappingAsync();
        
        if (!mapping.TryGetValue(category, out var categoryIcons))
            return null;

        // Try exact match first
        if (categoryIcons.TryGetValue(displayName, out var iconPath))
            return Path.Combine(_iconsPath, iconPath);

        // Try with .png extension
        var displayNameWithExtension = displayName.EndsWith(".png") ? displayName : $"{displayName}.png";
        if (categoryIcons.TryGetValue(displayNameWithExtension, out iconPath))
            return Path.Combine(_iconsPath, iconPath);

        return null;
    }

    /// <summary>
    /// Gets icon data as byte array
    /// </summary>
    public async Task<byte[]?> GetIconDataAsync(string category, string displayName)
    {
        var iconPath = await GetIconPathAsync(category, displayName);
        
        if (iconPath == null || !File.Exists(iconPath))
            return null;

        return await File.ReadAllBytesAsync(iconPath);
    }

    /// <summary>
    /// Gets icon data as a stream
    /// </summary>
    public async Task<Stream?> GetIconStreamAsync(string category, string displayName)
    {
        var iconPath = await GetIconPathAsync(category, displayName);
        
        if (iconPath == null || !File.Exists(iconPath))
            return null;

        return File.OpenRead(iconPath);
    }

    /// <summary>
    /// Gets all available icon categories
    /// </summary>
    public async Task<List<string>> GetCategoriesAsync()
    {
        var mapping = await LoadIconMappingAsync();
        return mapping.Keys.ToList();
    }

    /// <summary>
    /// Gets all icons in a specific category
    /// </summary>
    public async Task<Dictionary<string, string>> GetIconsInCategoryAsync(string category)
    {
        var mapping = await LoadIconMappingAsync();
        
        if (!mapping.TryGetValue(category, out var categoryIcons))
            return new Dictionary<string, string>();

        // Convert relative paths to absolute paths
        return categoryIcons.ToDictionary(
            kvp => kvp.Key, 
            kvp => Path.Combine(_iconsPath, kvp.Value)
        );
    }

    /// <summary>
    /// Checks if an icon exists for the given category and display name
    /// </summary>
    public async Task<bool> IconExistsAsync(string category, string displayName)
    {
        var iconPath = await GetIconPathAsync(category, displayName);
        return iconPath != null && File.Exists(iconPath);
    }

    /// <summary>
    /// Searches for icons by partial name match
    /// </summary>
    public async Task<List<IconInfo>> SearchIconsAsync(string searchTerm, string? category = null)
    {
        var mapping = await LoadIconMappingAsync();
        var results = new List<IconInfo>();

        var categoriesToSearch = category != null ? new[] { category } : mapping.Keys.ToArray();

        foreach (var cat in categoriesToSearch)
        {
            if (!mapping.TryGetValue(cat, out var categoryIcons))
                continue;

            foreach (var icon in categoryIcons)
            {
                if (icon.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new IconInfo
                    {
                        Category = cat,
                        DisplayName = icon.Key,
                        RelativePath = icon.Value,
                        FullPath = Path.Combine(_iconsPath, icon.Value)
                    });
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Searches for an icon across multiple categories (useful for items that can be in multiple categories)
    /// </summary>
    public async Task<string?> GetIconPathFromCategoriesAsync(IEnumerable<string> categories, string displayName)
    {
        foreach (var category in categories)
        {
            var iconPath = await GetIconPathAsync(category, displayName);
            if (iconPath != null)
                return iconPath;
        }
        return null;
    }

    /// <summary>
    /// Gets icon data across multiple categories
    /// </summary>
    public async Task<byte[]?> GetIconDataFromCategoriesAsync(IEnumerable<string> categories, string displayName)
    {
        foreach (var category in categories)
        {
            var iconData = await GetIconDataAsync(category, displayName);
            if (iconData != null)
                return iconData;
        }
        return null;
    }

    /// <summary>
    /// Checks if an icon exists in any of the specified categories
    /// </summary>
    public async Task<bool> IconExistsInCategoriesAsync(IEnumerable<string> categories, string displayName)
    {
        foreach (var category in categories)
        {
            if (await IconExistsAsync(category, displayName))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all available icon categories
    /// </summary>
    public async Task<IEnumerable<string>> GetAvailableCategoriesAsync()
    {
        var mapping = await LoadIconMappingAsync();
        return mapping.Keys;
    }

    /// <summary>
    /// Clears the cached icon mapping
    /// </summary>
    public void ClearCache()
    {
        _iconMapping = null;
    }
}

/// <summary>
/// Information about an icon
/// </summary>
public class IconInfo
{
    public string Category { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
}