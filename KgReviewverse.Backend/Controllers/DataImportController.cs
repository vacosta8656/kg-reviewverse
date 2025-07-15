using Microsoft.AspNetCore.Mvc;
using KgReviewverse.Backend.Services.Interfaces;

namespace KgReviewverse.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataImportController : ControllerBase
{
    private readonly IDataImportService _dataImportService;

    public DataImportController(IDataImportService dataImportService)
    {
        _dataImportService = dataImportService;
    }

    [HttpPost("import-from-file")]
    public async Task<IActionResult> ImportFromFile([FromBody] ImportFromFileRequest request)
    {
        try
        {
            var importedCount = await _dataImportService.ImportDramasFromFileAsync(request.FilePath);
            return Ok(new { ImportedCount = importedCount, Message = $"Successfully imported {importedCount} dramas" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("import-from-url")]
    public async Task<IActionResult> ImportFromUrl([FromBody] ImportFromUrlRequest request)
    {
        try
        {
            var importedCount = await _dataImportService.ImportDramasFromUrlAsync(request.Url);
            return Ok(new { ImportedCount = importedCount, Message = $"Successfully imported {importedCount} dramas" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("import-from-json")]
    public async Task<IActionResult> ImportFromJson([FromBody] ImportFromJsonRequest request)
    {
        try
        {
            var importedCount = await _dataImportService.ImportDramasFromJsonAsync(request.JsonContent);
            return Ok(new { ImportedCount = importedCount, Message = $"Successfully imported {importedCount} dramas" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public class ImportFromFileRequest
{
    public string FilePath { get; set; } = string.Empty;
}

public class ImportFromUrlRequest
{
    public string Url { get; set; } = string.Empty;
}

public class ImportFromJsonRequest
{
    public string JsonContent { get; set; } = string.Empty;
}
