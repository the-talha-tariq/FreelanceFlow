using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Invoices;

public class InvoiceDetailDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public Guid? MilestoneId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? PdfPath { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();

    /// <summary>Sum of all recorded payments — full Payments CRUD lands in the next step.</summary>
    public decimal AmountPaid { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}