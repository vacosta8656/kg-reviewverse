using System.Text.Json.Serialization;

namespace KgReviewverse.Common.Models.DTOs;

public class ScrapedDramaDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("categories")]
    public string[] Categories { get; set; } = Array.Empty<string>();
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("releaseDate")]
    public string ReleaseDate { get; set; } = string.Empty;
    
    [JsonPropertyName("coverImageUrl")]
    public string CoverImageUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("year")]
    public int Year { get; set; }
    
    [JsonPropertyName("sourceUrl")]
    public string SourceUrl { get; set; } = string.Empty;
}
