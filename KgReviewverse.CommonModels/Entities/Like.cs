using System;

namespace KgReviewverse.Common.Models.Entities;

public class Like
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
