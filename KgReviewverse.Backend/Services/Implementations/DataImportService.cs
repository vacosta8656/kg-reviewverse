using System.Text.Json;
using KgReviewverse.Backend.Services.Interfaces;
using KgReviewverse.Backend.Repositories.Interfaces;
using KgReviewverse.Common.Models.DTOs;
using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Services.Implementations;

public class DataImportService : IDataImportService
{
    private readonly IContentRepository _contentRepository;
    private readonly HttpClient _httpClient;

    public DataImportService(IContentRepository contentRepository, HttpClient httpClient)
    {
        _contentRepository = contentRepository;
        _httpClient = httpClient;
    }

    public async Task<int> ImportDramasFromJsonAsync(string jsonContent)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var scrapedDramas = JsonSerializer.Deserialize<ScrapedDramaDto[]>(jsonContent, options);
        if (scrapedDramas == null) return 0;

        int importedCount = 0;

        foreach (var scrapedDrama in scrapedDramas)
        {
            // Check if content already exists (you might want to check by title + year)
            var existingContent = await _contentRepository.GetContentByTitleAsync(scrapedDrama.Title);
            
            if (existingContent == null)
            {
                var content = new Content
                {
                    Title = scrapedDrama.Title,
                    Category = scrapedDrama.Categories.FirstOrDefault() ?? "Drama",
                    Description = scrapedDrama.Description != "N/A" ? scrapedDrama.Description : null,
                    ReleaseDate = ParseReleaseDate(scrapedDrama.ReleaseDate),
                    CoverImageUrl = scrapedDrama.CoverImageUrl != "N/A" ? scrapedDrama.CoverImageUrl : null,
                    CreatedAt = DateTime.UtcNow
                };

                await _contentRepository.AddContentAsync(content);
                importedCount++;
            }
        }

        return importedCount;
    }

    public async Task<int> ImportDramasFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var jsonContent = await File.ReadAllTextAsync(filePath);
        return await ImportDramasFromJsonAsync(jsonContent);
    }

    public async Task<int> ImportDramasFromUrlAsync(string url)
    {
        var jsonContent = await _httpClient.GetStringAsync(url);
        return await ImportDramasFromJsonAsync(jsonContent);
    }

    private DateTime? ParseReleaseDate(string releaseDateString)
    {
        if (string.IsNullOrWhiteSpace(releaseDateString) || releaseDateString == "N/A")
            return null;

        // Try to parse various date formats that might come from Wikipedia
        var formats = new[]
        {
            "yyyy-MM-dd",
            "MMMM d, yyyy",
            "MMMM yyyy",
            "yyyy"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(releaseDateString.Trim(), format, null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
        }

        // If all else fails, try general parsing
        if (DateTime.TryParse(releaseDateString, out var generalDate))
        {
            return generalDate;
        }

        return null;
    }
}
