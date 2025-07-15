using DotNetEnv;
using KgReviewverse.Crawler.Config;
using KgReviewverse.Crawler.Services;

Env.Load();

var config = new AppConfig
{
    GithubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "YOUR_GITHUB_TOKEN_HERE",
    GithubOwner = Environment.GetEnvironmentVariable("GITHUB_OWNER") ?? "YOUR_USERNAME",
    GithubRepo = Environment.GetEnvironmentVariable("GITHUB_REPO") ?? "korean-dramas-data",
    GithubBranch = Environment.GetEnvironmentVariable("GITHUB_BRANCH") ?? "main",
    StartYear = int.TryParse(Environment.GetEnvironmentVariable("START_YEAR"), out var startYear) ? startYear : 2021,
    EndYear = int.TryParse(Environment.GetEnvironmentVariable("END_YEAR"), out var endYear) ? endYear : 2025
};

var httpClient = new HttpClient();
var scraper = new DramaScraper(httpClient, config);
var storage = new DataStorage();
var uploader = new GitHubUploader(httpClient, config);

try
{
    var dramas = await scraper.ScrapeAllDramasAsync();

    if (dramas.Count == 0)
    {
        Console.WriteLine("No dramas found to process.");
        return;
    }

    var fileName = $"korean_dramas_{DateTime.Now:yyyyMMdd_HHmmss}.json";
    var jsonContent = await storage.SaveToJsonAsync(dramas);

    var githubUrl = await uploader.UploadJsonAsync(fileName, jsonContent);

    if (githubUrl != null)
    {
        Console.WriteLine($"\nProcess completed successfully!");
        Console.WriteLine($"{dramas.Count} dramas processed");
        Console.WriteLine($"GitHub URL: {githubUrl}");
        Console.WriteLine($"\nYou can now use this URL in your backend to import the data:");
        Console.WriteLine($"POST http://localhost:5044/api/dataimport/import-from-url");
        Console.WriteLine($"Body: {{ \"url\": \"{githubUrl}\" }}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
finally
{
    httpClient.Dispose();
}