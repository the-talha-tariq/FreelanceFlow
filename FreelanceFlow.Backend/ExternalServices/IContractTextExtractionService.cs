namespace FreelanceFlow.Backend.ExternalServices;

public interface IContractTextExtractionService
{
    /// <summary>
    /// Reads the uploaded contract document from disk and returns its plain
    /// text. Throws NotSupportedException for file types that can't be
    /// parsed yet (only .pdf and .txt are supported) and
    /// FileNotFoundException if the stored path no longer exists on disk.
    /// </summary>
    Task<string> ExtractTextAsync(string relativeDocumentPath);
}