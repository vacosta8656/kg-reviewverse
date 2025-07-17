using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Services.Interfaces;
using KgReviewverse.Common.Models.Utils;

namespace KgReviewverse.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("contents")]
    public async Task<IActionResult> GetAllContents()
    {
        var contents = await _contentService.GetAllContentsAsync();
        if (contents == null || !contents.Any())
        {
            return NotFound("No contents found.");
        }
        return Ok(contents);
    }

    [HttpGet("details/{contentId}")]
    public async Task<IActionResult> GetContentDetails(int contentId)
    {
        var content = await _contentService.GetContentDetailsAsync(contentId);
        if (content == null)
        {
            return NotFound($"Content with ID {contentId} not found.");
        }
        return Ok(content);
    }

    [HttpPost("import-from-url")]
    public async Task<IActionResult> ImportContentsFromUrl([FromBody] ImportUrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("URL is required.");

        using var httpClient = new HttpClient();
        try
        {
            var json = await httpClient.GetStringAsync(request.Url);
            var importContents = System.Text.Json.JsonSerializer.Deserialize<List<ImportContentDto>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (importContents == null || !importContents.Any())
                return BadRequest("No contents found in the provided JSON.");

            int inserted = 0;
            foreach (var importContent in importContents)
            {
                if (importContent == null || string.IsNullOrWhiteSpace(importContent.Title))
                {
                    continue;
                }

                var existing = await _contentService.GetContentByTitleAsync(importContent.Title);
                if (existing != null)
                    continue;

                var content = new Content
                {
                    Title = importContent.Title,
                    Description = importContent.Description,
                    ReleaseDate = importContent.ReleaseDate,
                    EndDate = importContent.ReleaseDate,
                    CoverImageUrl = importContent.CoverImageUrl,
                    CreatedAt = importContent.CreatedAt,
                    ContentCategories = new List<ContentCategory>()
                };

                foreach (var categoryName in importContent.Categories.Distinct())
                {
                    var category = await _contentService.GetOrCreateCategoryByNameAsync(categoryName);

                    content.ContentCategories.Add(new ContentCategory
                    {
                        Content = content,
                        Category = category
                    });
                }

                await _contentService.AddContentAsync(content);
                inserted++;
            }

            return Ok($"{inserted} new contents imported.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error importing contents: {ex.Message}");
        }
    }
}