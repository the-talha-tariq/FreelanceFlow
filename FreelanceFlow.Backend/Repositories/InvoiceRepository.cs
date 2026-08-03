using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Data;
using FreelanceFlow.Backend.Models.Entities;
using FreelanceFlow.Backend.Models.Enums;
using FreelanceFlow.Backend.Repositories.Interfaces;

namespace FreelanceFlow.Backend.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Invoice>> GetByFreelancerIdAsync(Guid freelancerId) =>
        await DbSet.Where(i => i.FreelancerId == freelancerId)
            .Include(i => i.Client)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();

    public async Task<Invoice?> GetWithDetailsAsync(Guid id) =>
        await DbSet
            .Include(i => i.Freelancer)
            .Include(i => i.Client)
            .Include(i => i.LineItems)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IReadOnlyList<Invoice>> GetOverdueAsync() =>
        await DbSet.Where(i =>
                i.Status != InvoiceStatus.Paid &&
                i.Status != InvoiceStatus.Cancelled &&
                i.DueDate < DateTime.UtcNow)
            .ToListAsync();

    public async Task<int> GetInvoiceCountForFreelancerAsync(Guid freelancerId) =>
        await DbSet.CountAsync(i => i.FreelancerId == freelancerId);
}