using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IContractRepository : IGenericRepository<Contract>
{
    Task<IReadOnlyList<Contract>> GetByFreelancerIdAsync(Guid freelancerId);

    /// <summary>Loads a contract together with its milestones and risk analyses.</summary>
    Task<Contract?> GetWithDetailsAsync(Guid id);
}