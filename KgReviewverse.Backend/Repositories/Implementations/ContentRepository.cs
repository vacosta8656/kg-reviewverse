using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using KgReviewverse.Backend.Data;

public class ContentRepository : IContentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ContentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Content>> GetAllContentsAsync()
    {
        return await _dbContext.Contents
            .OrderByDescending(c => c.ReleaseDate)
            .ToListAsync();
    }

    public async Task<Content?> GetContentDetailsAsync(int contentId)
    {
        return await _dbContext.Contents
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == contentId);
    }

    public async Task<double> GetAverageRatingAsync(int contentId)
    {
        return await _dbContext.Reviews
            .Where(r => r.ContentId == contentId)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;
    }

    public async Task<Content?> GetContentByTitleAsync(string title)
    {
        return await _dbContext.Contents.FirstOrDefaultAsync(c => c.Title == title);
    }

    public async Task AddContentAsync(Content content)
    {
        _dbContext.Contents.Add(content);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryByNameAsync(string name)
    {
        return await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task AddCategoryAsync(Category category)
    {
        _dbContext.Set<Category>().Add(category);
        await _dbContext.SaveChangesAsync();
    }
}
