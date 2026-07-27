using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.Models.Entities;

public class Invoice
{
    public Guid Id { get; set; }

    public Guid FreelancerId { get; set; }
    public ApplicationUser Freelancer { get; set; } = null!;

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    /// <summary>Null for manually created invoices not tied to a milestone.</summary>
    public Guid? MilestoneId { get; set; }
    public Milestone? Milestone { get; set; }

    /// <summary>Auto-incremented display number, e.g. INV-0001.</summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }

    /// <summary>Path to the generated PDF, e.g. /wwwroot/invoices/{id}.pdf.</summary>
    public string? PdfPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}