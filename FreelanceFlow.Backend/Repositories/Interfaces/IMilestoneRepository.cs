using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IMilestoneRepository : IGenericRepository<Milestone>
{
    Task<IReadOnlyList<Milestone>> GetByContractIdAsync(Guid contractId);

    /// <summary>Milestone plus its parent Contract, needed when generating an invoice.</summary>
    Task<Milestone?> GetWithContractAsync(Guid id);
}