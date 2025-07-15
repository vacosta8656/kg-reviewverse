using System;
using System.Collections.Generic;

namespace KgReviewverse.Common.Models.Entities;

public class Content
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();
}
