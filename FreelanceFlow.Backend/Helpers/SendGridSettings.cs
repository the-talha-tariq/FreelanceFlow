namespace FreelanceFlow.Backend.Helpers;

public class SendGridSettings
{
    /// <summary>
    /// Left blank in appsettings by default. When empty, sending an invoice
    /// logs the email instead of actually calling SendGrid, so the flow can
    /// be tested without an API key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@freelanceflow.local";
}