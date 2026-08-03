using FreelanceFlow.Backend.ExternalServices.Models;

namespace FreelanceFlow.Backend.ExternalServices;

public interface IOpenAIContractAnalyzerService
{
    /// <summary>
    /// Sends the extracted contract text to the AI and returns the flagged
    /// clauses. Returns deterministic mock data instead of calling OpenAI
    /// if no API key is configured (see OpenAISettings).
    /// </summary>
    Task<ContractAnalysisResult> AnalyzeContractTextAsync(string contractText);
}