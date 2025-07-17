namespace KgReviewverse.Common.Models.Entities;

public class ContentCategory
{
    public int ContentId { get; set; }
    public Content Content { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}