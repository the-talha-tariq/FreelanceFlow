using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<IReadOnlyList<Invoice>> GetByFreelancerIdAsync(Guid freelancerId);

    /// <summary>Invoice plus line items and payments.</summary>
    Task<Invoice?> GetWithDetailsAsync(Guid id);

    Task<IReadOnlyList<Invoice>> GetOverdueAsync();

    /// <summary>Used to generate the next sequential display number, e.g. INV-0042.</summary>
    Task<int> GetInvoiceCountForFreelancerAsync(Guid freelancerId);
}