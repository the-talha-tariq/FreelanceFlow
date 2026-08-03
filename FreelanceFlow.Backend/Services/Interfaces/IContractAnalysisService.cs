using FreelanceFlow.Backend.DTOs.Contracts;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

public interface IContractAnalysisService
{
    /// <summary>
    /// Extracts text from the contract's uploaded document, sends it to the
    /// AI, and replaces any previous analysis with the fresh set of flags.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<RiskAnalysisDto>>> AnalyzeAsync(Guid freelancerId, Guid contractId);

    /// <summary>Returns the most recently saved analysis for a contract.</summary>
    Task<ServiceResult<IReadOnlyList<RiskAnalysisDto>>> GetAnalysisAsync(Guid freelancerId, Guid contractId);
}