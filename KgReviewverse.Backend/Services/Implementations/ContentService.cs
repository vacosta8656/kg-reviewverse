using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Services.Interfaces;
using KgReviewverse.Backend.Repositories.Interfaces;

namespace KgReviewverse.Backend.Services.Implementations;

public class ContentService : IContentService
{
    private readonly IContentRepository _contentRepository;
    public ContentService(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public Task<IEnumerable<Content>> GetAllContentsAsync() => _contentRepository.GetAllContentsAsync();

    public Task<Content?> GetContentDetailsAsync(int contentId) => _contentRepository.GetContentDetailsAsync(contentId);

    public Task<double> GetAverageRatingAsync(int contentId) => _contentRepository.GetAverageRatingAsync(contentId);

    public async Task<Content?> GetContentByTitleAsync(string title)
    {
        return await _contentRepository.GetContentByTitleAsync(title);
    }

    public async Task AddContentAsync(Content content)
    {
        await _contentRepository.AddContentAsync(content);
    }

    public async Task<Category> GetOrCreateCategoryByNameAsync(string name)
{
    var category = await _contentRepository.GetCategoryByNameAsync(name);
    if (category == null)
    {
        category = new Category { Name = name };
        await _contentRepository.AddCategoryAsync(category);
    }
    return category;
}
}
