public class ImportContentDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Categories { get; set; } = new();
        
    public string SourceUrl { get; set; } = string.Empty;

}