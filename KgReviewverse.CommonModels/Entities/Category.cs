using System.Collections.Generic;

namespace KgReviewverse.Common.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<ContentCategory> ContentCategories { get; set; } = new List<ContentCategory>();
}