using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using FreelanceFlow.Backend.Services.Interfaces;

namespace FreelanceFlow.Backend.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _webRootPath;

    public FileStorageService(IWebHostEnvironment env)
    {
        // WebRootPath is null if wwwroot doesn't exist yet on disk (the API
        // template doesn't create one by default) — fall back to creating
        // it under ContentRootPath so this works out of the box.
        _webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        Directory.CreateDirectory(_webRootPath);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
    {
        var folderPath = Path.Combine(_webRootPath, subFolder);
        Directory.CreateDirectory(folderPath);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(folderPath, fileName);

        await using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Web-relative path, always forward slashes regardless of OS.
        return $"/{subFolder.Replace('\\', '/')}/{fileName}";
    }

    public void DeleteFile(string relativePath)
    {
        var trimmed = relativePath.TrimStart('/');
        var fullPath = Path.Combine(_webRootPath, trimmed);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
    public async Task<string> SaveBytesAsync(byte[] content, string subFolder, string fileName)
    {
        var folderPath = Path.Combine(_webRootPath, subFolder);
        Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);
        await File.WriteAllBytesAsync(fullPath, content);

        return $"/{subFolder.Replace('\\', '/')}/{fileName}";
    }
}