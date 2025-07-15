
using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Services.Interfaces;
using KgReviewverse.Backend.Repositories.Interfaces;

namespace KgReviewverse.Backend.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public Task AddOrUpdateReviewAsync(Review review) => _reviewRepository.AddOrUpdateReviewAsync(review);

    public Task<IEnumerable<Review>> GetReviewsByContentAsync(int contentId) => _reviewRepository.GetReviewsByContentAsync(contentId);
}
