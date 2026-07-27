using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IContractRiskAnalysisRepository : IGenericRepository<ContractRiskAnalysis>
{
    Task<IReadOnlyList<ContractRiskAnalysis>> GetByContractIdAsync(Guid contractId);
}