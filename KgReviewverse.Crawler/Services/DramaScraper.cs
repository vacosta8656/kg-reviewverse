using HtmlAgilityPack;
using KgReviewverse.Crawler.Models;
using KgReviewverse.Crawler.Config;
using Microsoft.Extensions.Logging;

namespace KgReviewverse.Crawler.Services;

public class DramaScraper
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;
    private readonly ILogger<DramaScraper> _logger;
    private const string BaseUrl = "https://en.wikipedia.org";

    public DramaScraper(HttpClient httpClient, AppConfig config, ILogger<DramaScraper> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<List<ScrapedDrama>> ScrapeAllDramasAsync()
    {
        var allDramas = new List<ScrapedDrama>();
        
        _logger.LogInformation("Starting Korean drama scraping...");
        _logger.LogInformation("Years: {StartYear}-{EndYear}", _config.StartYear, _config.EndYear);

        var yearLinks = await GetYearLinksAsync();
        
        foreach (var (yearText, yearUrl) in yearLinks)
        {
            var dramasForYear = await ScrapeYearAsync(yearText, yearUrl);
            allDramas.AddRange(dramasForYear);
            _logger.LogInformation("Found {DramaCount} dramas for {Year}", dramasForYear.Count, yearText);
        }

        return allDramas;
    }

    private async Task<List<(string yearText, string yearUrl)>> GetYearLinksAsync()
    {
        var indexUrl = $"{BaseUrl}/wiki/List_of_years_in_South_Korean_television";
        var indexHtml = await _httpClient.GetStringAsync(indexUrl);
        var indexDoc = new HtmlDocument();
        indexDoc.LoadHtml(indexHtml);

        var yearLinks = indexDoc.DocumentNode.SelectNodes("//dl//dd//a")
            ?.Where(a =>
            {
                var text = a.InnerText.Trim();
                return int.TryParse(text, out var year) && 
                       year >= _config.StartYear && 
                       year <= _config.EndYear;
            })
            .Select(a => (a.InnerText.Trim(), $"{BaseUrl}{a.GetAttributeValue("href", "")}"))
            .ToList() ?? new List<(string, string)>();

        if (!yearLinks.Any())
        {
            throw new InvalidOperationException($"No year links found between {_config.StartYear} and {_config.EndYear}.");
        }

        return yearLinks;
    }

    private async Task<List<ScrapedDrama>> ScrapeYearAsync(string yearText, string yearUrl)
    {
        var dramas = new List<ScrapedDrama>();

        try
        {
            var yearHtml = await _httpClient.GetStringAsync(yearUrl);
            var yearDoc = new HtmlDocument();
            yearDoc.LoadHtml(yearHtml);

            var dramaHeader = yearDoc.DocumentNode.SelectSingleNode("//h3[@id='Drama']/parent::*");
            if (dramaHeader == null)
            {
                _logger.LogWarning("Drama header not found for {Year}", yearText);
                return dramas;
            }

            var dramaTableNode = dramaHeader.SelectSingleNode("following-sibling::table[contains(@class,'wikitable')]");
            if (dramaTableNode == null)
            {
                _logger.LogWarning("Drama table not found for {Year}", yearText);
                return dramas;
            }

            var dramaRows = dramaTableNode.SelectNodes(".//tr[position()>1]");
            if (dramaRows == null)
            {
                _logger.LogWarning("No drama rows found for {Year}", yearText);
                return dramas;
            }

            foreach (var row in dramaRows)
            {
                var drama = await ScrapeDramaFromRowAsync(row, yearText);
                if (drama != null)
                {
                    dramas.Add(drama);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error processing year {Year}: {Error}", yearText, ex.Message);
        }

        return dramas;
    }

    private async Task<ScrapedDrama?> ScrapeDramaFromRowAsync(HtmlNode row, string yearText)
    {
        try
        {
            var titleNode = row.SelectSingleNode(".//td[1]//i//a");
            if (titleNode == null) return null;

            var dramaTitle = titleNode.InnerText.Trim();
            var dramaLink = titleNode.GetAttributeValue("href", "");
            var fullDramaUrl = $"{BaseUrl}{dramaLink}";

            var dramaHtml = await _httpClient.GetStringAsync(fullDramaUrl);
            var dramaDoc = new HtmlDocument();
            dramaDoc.LoadHtml(dramaHtml);

            var title = ExtractTitle(dramaDoc);
            var imageUrl = ExtractImageUrl(dramaDoc);
            var description = ExtractDescription(dramaDoc);
            var categories = ExtractCategories(dramaDoc);
            var releaseDate = ExtractReleaseDate(dramaDoc);

            return new ScrapedDrama
            {
                Title = title,
                Categories = categories.ToArray(),
                Description = description,
                ReleaseDate = releaseDate,
                CoverImageUrl = imageUrl,
                Year = int.Parse(yearText),
                SourceUrl = fullDramaUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Error processing drama: {Error}", ex.Message);
            return null;
        }
    }

    private static string ExtractTitle(HtmlDocument doc)
    {
        return doc.DocumentNode.SelectSingleNode("//table[contains(@class,'infobox')]//tr[1]//th//i")?.InnerText?.Trim() ??
               doc.DocumentNode.SelectSingleNode("//h1[@id='firstHeading']/i")?.InnerText?.Trim() ?? 
               "N/A";
    }

    private static string ExtractImageUrl(HtmlDocument doc)
    {
        var imageUrl = doc.DocumentNode
            .SelectSingleNode("//table[contains(@class,'infobox')]//tr//td[contains(@class,'infobox-image')]//span//a//img")
            ?.GetAttributeValue("src", "") ?? "";

        if (!imageUrl.StartsWith("http") && !string.IsNullOrEmpty(imageUrl))
        {
            imageUrl = $"https:{imageUrl}";
        }

        return string.IsNullOrEmpty(imageUrl) ? "N/A" : imageUrl;
    }

    private static string ExtractDescription(HtmlDocument doc)
    {
        var plotHeader = doc.DocumentNode
            .SelectSingleNode("//h2[@id='Plot']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//h2[@id='Synopsis']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//h2[@id='Premise']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//h2[@id='Summary']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//h2[@id='Overview']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//h2[@id='Format']/parent::*") ??
            doc.DocumentNode.SelectSingleNode("//table[contains(@class,'infobox')]");

        return plotHeader?.SelectSingleNode("following-sibling::p")?.InnerText?.Trim() ?? "N/A";
    }

    private static List<string> ExtractCategories(HtmlDocument doc)
    {
        var categories = new List<string>();
        var categoryTd = doc.DocumentNode.SelectSingleNode("//td[contains(@class,'infobox-data category')]");

        if (categoryTd != null)
        {
            var liNodes = categoryTd.SelectNodes(".//ul/li");
            if (liNodes != null)
            {
                categories = liNodes
                    .Select(li => li.InnerText.Trim())
                    .Where(text => !string.IsNullOrWhiteSpace(text))
                    .ToList();
            }
            else
            {
                var singleCategory = categoryTd.InnerText.Trim();
                if (!string.IsNullOrWhiteSpace(singleCategory))
                {
                    categories.Add(singleCategory);
                }
            }
        }

        return categories;
    }

    private static string ExtractReleaseDate(HtmlDocument doc)
    {
        var releaseDateNode = doc.DocumentNode
            .SelectSingleNode("//table[contains(@class,'infobox')]//th[contains(text(), 'Release')]/following-sibling::td") ??
            doc.DocumentNode.SelectSingleNode("//table[contains(@class,'infobox')]//th[contains(text(), 'release')]/following-sibling::td") ??
            doc.DocumentNode.SelectSingleNode("//table[contains(@class,'infobox')]//div[contains(text(), 'Release')]/parent::*/following-sibling::td");

        return releaseDateNode?.InnerText?.Trim() ?? "N/A";
    }
}
