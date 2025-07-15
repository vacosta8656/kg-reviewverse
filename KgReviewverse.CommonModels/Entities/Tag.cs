using System.Collections.Generic;

namespace KgReviewverse.Common.Models.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<ContentTag> ContentTags { get; set; } = new List<ContentTag>();
}
