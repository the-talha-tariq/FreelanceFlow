using Microsoft.AspNetCore.Http;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Saves the uploaded file under wwwroot/{subFolder}/{a new guid file name},
    /// returning the web-relative path (e.g. "/contracts/{id}/abc123.pdf") to
    /// store on the owning entity.
    /// </summary>
    Task<string> SaveFileAsync(IFormFile file, string subFolder);

    void DeleteFile(string relativePath);
}