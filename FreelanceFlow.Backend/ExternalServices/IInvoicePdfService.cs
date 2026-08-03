using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.ExternalServices;

public interface IInvoicePdfService
{
    /// <summary>
    /// Renders the invoice (with Freelancer, Client, and LineItems already
    /// loaded) to a PDF and returns the raw bytes.
    /// </summary>
    byte[] GeneratePdf(Invoice invoice);
}