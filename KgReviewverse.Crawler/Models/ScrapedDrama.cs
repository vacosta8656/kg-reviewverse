namespace KgReviewverse.Crawler.Models;

public class ScrapedDrama
{
    public string Title { get; set; } = string.Empty;
    public string[] Categories { get; set; } = Array.Empty<string>();
    public string Description { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int Year { get; set; }
    public string SourceUrl { get; set; } = string.Empty;
}
