using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using FreelanceFlow.Backend.Helpers;

namespace FreelanceFlow.Backend.ExternalServices;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _settings;
    private readonly ILogger<SendGridEmailService> _logger;

    public SendGridEmailService(IOptions<SendGridSettings> settings, ILogger<SendGridEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendInvoiceEmailAsync(
        string toEmail, string clientName, string invoiceNumber, decimal totalAmount, string currency, byte[] pdfBytes)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            // No SendGrid key configured — log instead of sending so the
            // full invoice/send flow can still be tested end to end.
            _logger.LogInformation(
                "SendGrid:ApiKey not configured — invoice {InvoiceNumber} for {ClientName} <{ToEmail}> " +
                "({Total} {Currency}) was NOT emailed, just logged.",
                invoiceNumber, clientName, toEmail, totalAmount, currency);
            return;
        }

        var client = new SendGridClient(_settings.ApiKey);
        var from = new EmailAddress(_settings.FromEmail, "FreelanceFlow");
        var to = new EmailAddress(toEmail, clientName);

        var message = MailHelper.CreateSingleEmail(
            from,
            to,
            subject: $"Invoice {invoiceNumber} — {totalAmount:0.00} {currency}",
            plainTextContent:
                $"Hi {clientName},\n\nPlease find attached invoice {invoiceNumber} for {totalAmount:0.00} {currency}.\n\nThank you.",
            htmlContent:
                $"<p>Hi {clientName},</p><p>Please find attached invoice <strong>{invoiceNumber}</strong> " +
                $"for <strong>{totalAmount:0.00} {currency}</strong>.</p><p>Thank you.</p>");

        message.AddAttachment($"{invoiceNumber}.pdf", Convert.ToBase64String(pdfBytes), "application/pdf");

        var response = await client.SendEmailAsync(message);

        if ((int)response.StatusCode >= 300)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new InvalidOperationException($"SendGrid returned {response.StatusCode}: {body}");
        }
    }
}