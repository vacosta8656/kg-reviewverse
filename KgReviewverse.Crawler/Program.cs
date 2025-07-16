using DotNetEnv;
using KgReviewverse.Crawler.Config;
using KgReviewverse.Crawler.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Env.Load();

var config = new AppConfig
{
    GithubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "",
    GithubOwner = Environment.GetEnvironmentVariable("GITHUB_OWNER") ?? "",
    GithubRepo = Environment.GetEnvironmentVariable("GITHUB_REPO") ?? "",
    GithubBranch = Environment.GetEnvironmentVariable("GITHUB_BRANCH") ?? "",
    StartYear = int.TryParse(Environment.GetEnvironmentVariable("START_YEAR"), out var startYear) ? startYear : 2021,
    EndYear = int.TryParse(Environment.GetEnvironmentVariable("END_YEAR"), out var endYear) ? endYear : 2025
};

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole(options =>
    {
        options.FormatterName = "simple";
    });
    builder.SetMinimumLevel(LogLevel.Information);
    
    builder.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
    builder.AddFilter("Microsoft", LogLevel.Warning);
    builder.AddFilter("System", LogLevel.Warning);
});

services.AddSingleton(config);
services.AddHttpClient();
services.AddTransient<DramaScraper>();
services.AddTransient<DataFormatter>();
services.AddTransient<GitHubUploader>();

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting KgReviewverse Crawler...");
    
    var scraper = serviceProvider.GetRequiredService<DramaScraper>();
    var storage = serviceProvider.GetRequiredService<DataFormatter>();
    var uploader = serviceProvider.GetRequiredService<GitHubUploader>();

    var dramas = await scraper.ScrapeAllDramasAsync();

    if (dramas.Count == 0)
    {
        logger.LogWarning("No dramas found to process.");
        return;
    }

    var fileName = $"korean_dramas.json";
    var jsonContent = storage.SaveToJson(dramas);

    var githubUrl = await uploader.UploadJsonAsync(fileName, jsonContent);

    if (githubUrl != null)
    {
        logger.LogInformation("Process completed successfully!");
        logger.LogInformation("{DramaCount} dramas processed", dramas.Count);
        logger.LogInformation("GitHub URL: {GitHubUrl}", githubUrl);
        logger.LogInformation("You can now use this URL in your backend to import the data:");
        logger.LogInformation("POST http://localhost:5044/api/dataimport/import-from-url");
        logger.LogInformation("Body: {{ \"url\": \"{GitHubUrl}\" }}", githubUrl);
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during crawling");
}
finally
{
    serviceProvider.Dispose();
}