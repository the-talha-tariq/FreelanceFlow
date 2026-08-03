using FreelanceFlow.Backend.DTOs.Invoices;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.Services.Interfaces;

/// <summary>Generated PDF bytes plus the filename to serve them under.</summary>
public record InvoicePdfResult(byte[] Bytes, string FileName);

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceDto>> GetAllForFreelancerAsync(Guid freelancerId);
    Task<ServiceResult<InvoiceDetailDto>> GetByIdAsync(Guid freelancerId, Guid invoiceId);
    Task<ServiceResult<InvoiceDetailDto>> CreateAsync(Guid freelancerId, CreateInvoiceDto dto);
    Task<ServiceResult<InvoiceDetailDto>> UpdateAsync(Guid freelancerId, Guid invoiceId, UpdateInvoiceDto dto);
    Task<ServiceResult<bool>> DeleteAsync(Guid freelancerId, Guid invoiceId);
    Task<ServiceResult<InvoicePdfResult>> GetPdfAsync(Guid freelancerId, Guid invoiceId);

    /// <summary>Emails the invoice to the client and marks it Sent if it was still Draft.</summary>
    Task<ServiceResult<InvoiceDetailDto>> SendAsync(Guid freelancerId, Guid invoiceId);
}