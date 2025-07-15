using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Services.Interfaces;

public interface IContentService
{
    Task<IEnumerable<Content>> GetAllContentsAsync();
    Task<Content?> GetContentDetailsAsync(int contentId);
    Task<double> GetAverageRatingAsync(int contentId);
}