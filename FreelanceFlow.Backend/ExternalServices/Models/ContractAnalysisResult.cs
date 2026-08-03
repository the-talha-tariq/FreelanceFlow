namespace FreelanceFlow.Backend.ExternalServices.Models;

public record ContractAnalysisResult(List<ClauseFlag> Flags, string RawResponse);