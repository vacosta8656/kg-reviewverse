using System.Text;
using System.Text.Json;
using KgReviewverse.Crawler.Config;

namespace KgReviewverse.Crawler.Services;

public class GitHubUploader
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;

    public GitHubUploader(HttpClient httpClient, AppConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string?> UploadJsonAsync(string fileName, string jsonContent)
    {
        if (!_config.IsGithubConfigured)
        {
            Console.WriteLine("⚠️ GitHub token not configured. Skipping upload.");
            Console.WriteLine("To enable GitHub upload:");
            Console.WriteLine("1. Create a GitHub repository");
            Console.WriteLine("2. Get a Personal Access Token");
            Console.WriteLine("3. Update the .env file with your GitHub configuration");
            return null;
        }

        try
        {
            Console.WriteLine("\n📤 Uploading to GitHub...");
            
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
                
                Console.WriteLine("✅ Successfully uploaded to GitHub!");
                Console.WriteLine($"📍 File URL: {downloadUrl}");
                Console.WriteLine($"🔗 Raw URL for import: {downloadUrl}");
                
                return downloadUrl;
            }
            else
            {
                var error = await uploadResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ GitHub upload failed: {uploadResponse.StatusCode}");
                Console.WriteLine($"Error: {error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error uploading to GitHub: {ex.Message}");
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
