using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Repositories.Interfaces;
using KgReviewverse.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace KgReviewverse.Backend.Repositories.Implementations;

public class ContentRepository : IContentRepository
{
    private readonly ApplicationDbContext _context;
    public ContentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Content>> GetAllContentsAsync()
    {
        return await _context.Contents
            .OrderByDescending(c => c.ReleaseDate)
            .ToListAsync();
    }

    public async Task<Content?> GetContentDetailsAsync(int contentId)
    {
        return await _context.Contents
            .Include(c => c.ContentTags)
                .ThenInclude(ct => ct.Tag)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == contentId);
    }

    public async Task<Content?> GetContentByTitleAsync(string title)
    {
        return await _context.Contents
            .FirstOrDefaultAsync(c => c.Title.ToLower() == title.ToLower());
    }

    public async Task<Content> AddContentAsync(Content content)
    {
        _context.Contents.Add(content);
        await _context.SaveChangesAsync();
        return content;
    }

    public async Task<double> GetAverageRatingAsync(int contentId)
    {
        return await _context.Reviews
            .Where(r => r.ContentId == contentId)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;
    }
}
