using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.ExternalServices.Models;

/// <summary>
/// One flagged clause, as parsed from the AI's JSON response (or from the
/// mock generator when no API key is configured). Not exposed over the API
/// directly — ContractAnalysisService turns these into ContractRiskAnalysis
/// rows, which get mapped to RiskAnalysisDto for the controller.
/// </summary>
public record ClauseFlag(
    ClauseType ClauseType,
    Severity Severity,
    string ExtractedText,
    string Explanation,
    string SuggestedAlternative);