using KgReviewverse.Common.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KgReviewverse.Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ContentTag> ContentTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentTag>()
            .HasKey(ct => new { ct.ContentId, ct.TagId });

        modelBuilder.Entity<ContentTag>()
            .HasOne(ct => ct.Content)
            .WithMany(c => c.ContentTags)
            .HasForeignKey(ct => ct.ContentId);

        modelBuilder.Entity<ContentTag>()
            .HasOne(ct => ct.Tag)
            .WithMany(t => t.ContentTags)
            .HasForeignKey(ct => ct.TagId);
    }
}
