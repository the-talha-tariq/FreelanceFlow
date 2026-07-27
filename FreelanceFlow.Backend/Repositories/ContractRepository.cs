using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class ContractRepository : GenericRepository<Contract>, IContractRepository
{
    public ContractRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Contract>> GetByFreelancerIdAsync(Guid freelancerId) =>
        await DbSet.Where(c => c.FreelancerId == freelancerId)
            .Include(c => c.Client)
            .ToListAsync();

    public async Task<Contract?> GetWithDetailsAsync(Guid id) =>
        await DbSet
            .Include(c => c.Client)
            .Include(c => c.Milestones)
            .Include(c => c.RiskAnalyses)
            .FirstOrDefaultAsync(c => c.Id == id);
}