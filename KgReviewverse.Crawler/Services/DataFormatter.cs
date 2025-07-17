using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace KgReviewverse.Crawler.Services;

public class DataFormatter
{
    private readonly ILogger<DataFormatter> _logger;

    public DataFormatter(ILogger<DataFormatter> logger)
    {
        _logger = logger;
    }

    public string SaveToJson(List<ImportContentDto> dramas)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonString = JsonSerializer.Serialize(dramas, jsonOptions);
        var fileName = $"korean_dramas.json";
        
        _logger.LogInformation("=== SCRAPING COMPLETED ===");
        _logger.LogInformation("Total dramas scraped: {DramaCount}", dramas.Count);
        _logger.LogInformation("Data saved to: {FileName}", fileName);
        _logger.LogInformation("File location: {FilePath}", Path.GetFullPath(fileName));

        return jsonString;
    }
}
