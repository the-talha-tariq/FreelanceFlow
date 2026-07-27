using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<IReadOnlyList<Client>> GetByFreelancerIdAsync(Guid freelancerId);

    /// <summary>Fetches a client even if it's been soft-deleted (IgnoreQueryFilters).</summary>
    Task<Client?> GetByIdIncludingDeletedAsync(Guid id);
}