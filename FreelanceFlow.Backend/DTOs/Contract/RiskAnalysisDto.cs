using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Contracts;

public class RiskAnalysisDto
{
    public Guid Id { get; set; }
    public ClauseType ClauseType { get; set; }
    public Severity Severity { get; set; }
    public string ExtractedText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string SuggestedAlternative { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}