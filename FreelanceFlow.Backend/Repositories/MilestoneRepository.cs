using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class MilestoneRepository : GenericRepository<Milestone>, IMilestoneRepository
{
    public MilestoneRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Milestone>> GetByContractIdAsync(Guid contractId) =>
        await DbSet.Where(m => m.ContractId == contractId)
            .OrderBy(m => m.DueDate)
            .ToListAsync();

    public async Task<Milestone?> GetWithContractAsync(Guid id) =>
        await DbSet.Include(m => m.Contract)
            .ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(m => m.Id == id);
}