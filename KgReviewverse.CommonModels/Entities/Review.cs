using System;

namespace KgReviewverse.Common.Models.Entities;

public class Review
{
    public int Id { get; set; }

    public int ContentId { get; set; }
    public Content Content { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int Rating { get; set; }
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
