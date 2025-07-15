using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using KgReviewverse.Common.Models.Entities;
using KgReviewverse.Backend.Services.Interfaces;

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
}