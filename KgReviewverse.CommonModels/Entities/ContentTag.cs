namespace KgReviewverse.Common.Models.Entities;

public class ContentTag
{
    public int ContentId { get; set; }
    public Content Content { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
