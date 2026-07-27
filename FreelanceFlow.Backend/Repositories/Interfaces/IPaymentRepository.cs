using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(Guid invoiceId);
}