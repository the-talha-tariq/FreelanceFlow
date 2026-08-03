using System.Text;
using Microsoft.AspNetCore.Hosting;
using UglyToad.PdfPig;

namespace FreelanceFlow.Backend.ExternalServices;

public class ContractTextExtractionService : IContractTextExtractionService
{
    private static readonly string[] SupportedExtensions = { ".pdf", ".txt" };

    private readonly string _webRootPath;

    public ContractTextExtractionService(IWebHostEnvironment env)
    {
        _webRootPath = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
    }

    public async Task<string> ExtractTextAsync(string relativeDocumentPath)
    {
        var trimmed = relativeDocumentPath.TrimStart('/');
        var fullPath = Path.Combine(_webRootPath, trimmed);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("The contract document could not be found on disk.", fullPath);
        }

        var extension = Path.GetExtension(fullPath).ToLowerInvariant();

        if (!SupportedExtensions.Contains(extension))
        {
            throw new NotSupportedException(
                $"AI analysis doesn't support {extension} files yet — re-upload the contract as a PDF or .txt file. " +
                $"(Supported: {string.Join(", ", SupportedExtensions)})");
        }

        return extension == ".pdf"
            ? ExtractFromPdf(fullPath)
            : await File.ReadAllTextAsync(fullPath);
    }

    private static string ExtractFromPdf(string fullPath)
    {
        using var document = PdfDocument.Open(fullPath);

        var builder = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            builder.AppendLine(page.Text);
        }

        return builder.ToString();
    }
}