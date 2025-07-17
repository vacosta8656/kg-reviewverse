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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContentCategory>()
            .HasKey(cc => new { cc.ContentId, cc.CategoryId });

        modelBuilder.Entity<ContentCategory>()
            .HasOne(cc => cc.Content)
            .WithMany(c => c.ContentCategories)
            .HasForeignKey(cc => cc.ContentId);

        modelBuilder.Entity<ContentCategory>()
            .HasOne(cc => cc.Category)
            .WithMany(c => c.ContentCategories)
            .HasForeignKey(cc => cc.CategoryId);

    }
}
