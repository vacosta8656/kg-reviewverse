namespace KgReviewverse.Crawler.Config;

public class AppConfig
{
    public string GithubToken { get; set; } = string.Empty;
    public string GithubOwner { get; set; } = string.Empty;
    public string GithubRepo { get; set; } = string.Empty;
    public string GithubBranch { get; set; } = "main";
    public int StartYear { get; set; } = 2021;
    public int EndYear { get; set; } = 2025;

    public bool IsGithubConfigured => 
        !string.IsNullOrEmpty(GithubToken) && 
        GithubToken != "YOUR_GITHUB_TOKEN_HERE" &&
        !string.IsNullOrEmpty(GithubOwner) && 
        GithubOwner != "YOUR_USERNAME" &&
        !string.IsNullOrEmpty(GithubRepo);
}
