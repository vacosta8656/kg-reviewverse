using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Repositories.Interfaces;

public interface IReviewRepository
{
    Task AddOrUpdateReviewAsync(Review review);
    Task<IEnumerable<Review>> GetReviewsByContentAsync(int contentId);
}