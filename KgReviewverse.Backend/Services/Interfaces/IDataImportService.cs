using KgReviewverse.Common.Models.DTOs;
using KgReviewverse.Common.Models.Entities;

namespace KgReviewverse.Backend.Services.Interfaces;

public interface IDataImportService
{
    Task<int> ImportDramasFromJsonAsync(string jsonContent);
    Task<int> ImportDramasFromFileAsync(string filePath);
    Task<int> ImportDramasFromUrlAsync(string url);
}
