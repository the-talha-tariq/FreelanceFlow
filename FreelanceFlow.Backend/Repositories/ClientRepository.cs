using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Client>> GetByFreelancerIdAsync(Guid freelancerId) =>
        await DbSet.Where(c => c.FreelancerId == freelancerId).ToListAsync();

    public async Task<Client?> GetByIdIncludingDeletedAsync(Guid id) =>
        await DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
}