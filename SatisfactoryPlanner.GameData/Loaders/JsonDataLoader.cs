using System.Text.Json;

namespace SatisfactoryPlanner.GameData.Loaders;

/// <summary>
/// Generic JSON data loader that can load and cache any type from a JSON file
/// </summary>
public class JsonDataLoader<T> where T : class
{
    private readonly string _filePath;
    private T? _cachedData;

    public JsonDataLoader(string filePath)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Loads and deserializes data from the JSON file
    /// </summary>
    public async Task<T> LoadAsync()
    {
        if (_cachedData != null)
            return _cachedData;

        try
        {
            var jsonContent = await File.ReadAllTextAsync(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            _cachedData = JsonSerializer.Deserialize<T>(jsonContent, options);
            
            if (_cachedData == null)
                throw new InvalidOperationException($"Failed to deserialize data from {_filePath}");

            return _cachedData;
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"Data file not found: {_filePath}");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON in data file {_filePath}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Clears the cached data (useful for hot-reloading during development)
    /// </summary>
    public void ClearCache()
    {
        _cachedData = null;
    }
}