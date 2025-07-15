using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Repositories.Interfaces;
using KgReviewverse.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace KgReviewverse.Backend.Repositories.Implementations;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddOrUpdateReviewAsync(Review review)
    {
        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ContentId == review.ContentId && r.UserId == review.UserId);

        if (existingReview != null)
        {
            existingReview.Rating = review.Rating;
            existingReview.Text = review.Text;
            existingReview.CreatedAt = DateTime.UtcNow;
            _context.Reviews.Update(existingReview);
        }
        else
        {
            await _context.Reviews.AddAsync(review);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByContentAsync(int contentId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ContentId == contentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
