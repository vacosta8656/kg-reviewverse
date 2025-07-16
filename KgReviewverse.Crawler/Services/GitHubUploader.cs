using System.Text;
using System.Text.Json;
using KgReviewverse.Crawler.Config;
using Microsoft.Extensions.Logging;

namespace KgReviewverse.Crawler.Services;

public class GitHubUploader
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;
    private readonly ILogger<GitHubUploader> _logger;

    public GitHubUploader(HttpClient httpClient, AppConfig config, ILogger<GitHubUploader> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string?> UploadJsonAsync(string fileName, string jsonContent)
    {
        if (!_config.IsGithubConfigured)
        {
            _logger.LogWarning("GitHub token not configured. Skipping upload.");
            return null;
        }

        try
        {
            _logger.LogInformation("Uploading to GitHub...");
            
            ConfigureHttpClient();

            var sha = await GetExistingFileShaAsync(fileName);
            var uploadData = CreateUploadData(jsonContent, sha);
            var uploadUrl = $"https://api.github.com/repos/{_config.GithubOwner}/{_config.GithubRepo}/contents/{fileName}";

            var uploadJson = JsonSerializer.Serialize(uploadData);
            var uploadContent = new StringContent(uploadJson, Encoding.UTF8, "application/json");

            var uploadResponse = await _httpClient.PutAsync(uploadUrl, uploadContent);

            if (uploadResponse.IsSuccessStatusCode)
            {
                var responseContent = await uploadResponse.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var downloadUrl = responseJson.GetProperty("content").GetProperty("download_url").GetString();
                
                _logger.LogInformation("Successfully uploaded to GitHub!");
                _logger.LogInformation("File URL: {FileUrl}", downloadUrl);
                _logger.LogInformation("Raw URL for import: {RawUrl}", downloadUrl);
                
                return downloadUrl;
            }
            else
            {
                var error = await uploadResponse.Content.ReadAsStringAsync();
                _logger.LogError("GitHub upload failed: {StatusCode}", uploadResponse.StatusCode);
                _logger.LogError("Error: {Error}", error);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error uploading to GitHub: {Error}", ex.Message);
            return null;
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.GithubToken}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "KgReviewverse-Crawler");
    }

    private async Task<string?> GetExistingFileShaAsync(string fileName)
    {
        try
        {
            var checkUrl = $"https://api.github.com/repos/{_config.GithubOwner}/{_config.GithubRepo}/contents/{fileName}";
            var checkResponse = await _httpClient.GetAsync(checkUrl);
            
            if (checkResponse.IsSuccessStatusCode)
            {
                var existingContent = await checkResponse.Content.ReadAsStringAsync();
                var existingJson = JsonSerializer.Deserialize<JsonElement>(existingContent);
                return existingJson.GetProperty("sha").GetString();
            }
        }
        catch (Exception)
        {
            // File doesn't exist or other error, return null
        }

        return null;
    }

    private object CreateUploadData(string content, string? sha)
    {
        var uploadData = new
        {
            message = $"Update Korean dramas data - {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content)),
            branch = _config.GithubBranch
        };

        if (sha != null)
        {
            return new
            {
                uploadData.message,
                uploadData.content,
                uploadData.branch,
                sha = sha
            };
        }

        return uploadData;
    }
}
