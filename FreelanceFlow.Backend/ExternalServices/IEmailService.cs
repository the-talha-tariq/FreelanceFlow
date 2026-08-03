namespace FreelanceFlow.Backend.ExternalServices;

public interface IEmailService
{
    /// <summary>
    /// Emails the invoice PDF to the client. Logs instead of sending if
    /// SendGrid:ApiKey isn't configured (see SendGridSettings).
    /// </summary>
    Task SendInvoiceEmailAsync(
        string toEmail, string clientName, string invoiceNumber, decimal totalAmount, string currency, byte[] pdfBytes);
}