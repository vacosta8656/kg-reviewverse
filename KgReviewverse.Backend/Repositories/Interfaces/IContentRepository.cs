using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Repositories.Interfaces;

public interface IContentRepository
{
    Task<IEnumerable<Content>> GetAllContentsAsync();
    Task<Content?> GetContentDetailsAsync(int contentId);
    Task<double> GetAverageRatingAsync(int contentId);
}