using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.Models.Entities;

/// <summary>
/// One row per flagged clause. A single contract analysis run can produce
/// many rows here, all sharing the same ContractId and AnalyzedAt.
/// </summary>
public class ContractRiskAnalysis
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Full raw JSON response from OpenAI, kept for audit/debugging.</summary>
    public string RawAIResponse { get; set; } = string.Empty;

    public ClauseType ClauseType { get; set; }
    public Severity Severity { get; set; }
    public string ExtractedText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string SuggestedAlternative { get; set; } = string.Empty;
}