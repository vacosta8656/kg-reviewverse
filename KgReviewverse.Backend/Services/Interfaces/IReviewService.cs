using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Services.Interfaces;

public interface IReviewService
{
    Task AddOrUpdateReviewAsync(Review review);
    Task<IEnumerable<Review>> GetReviewsByContentAsync(int contentId);
}