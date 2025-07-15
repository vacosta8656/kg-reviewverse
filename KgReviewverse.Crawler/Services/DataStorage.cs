using System.Text.Json;
using KgReviewverse.Crawler.Models;

namespace KgReviewverse.Crawler.Services;

public class DataStorage
{
    public async Task<string> SaveToJsonAsync(List<ScrapedDrama> dramas)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonString = JsonSerializer.Serialize(dramas, jsonOptions);
        var fileName = $"korean_dramas_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        
        await File.WriteAllTextAsync(fileName, jsonString);
        
        Console.WriteLine($"\n=== SCRAPING COMPLETED ===");
        Console.WriteLine($"Total dramas scraped: {dramas.Count}");
        Console.WriteLine($"Data saved to: {fileName}");
        Console.WriteLine($"File location: {Path.GetFullPath(fileName)}");

        return jsonString;
    }
}
