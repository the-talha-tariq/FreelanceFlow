using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class ContractRiskAnalysisRepository : GenericRepository<ContractRiskAnalysis>, IContractRiskAnalysisRepository
{
    public ContractRiskAnalysisRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ContractRiskAnalysis>> GetByContractIdAsync(Guid contractId) =>
        await DbSet.Where(r => r.ContractId == contractId)
            .OrderByDescending(r => r.AnalyzedAt)
            .ToListAsync();
}