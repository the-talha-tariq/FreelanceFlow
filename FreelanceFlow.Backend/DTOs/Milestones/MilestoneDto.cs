using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Milestones;

public class MilestoneDto
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public MilestoneStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>Set once this milestone has spawned an invoice via /complete.</summary>
    public Guid? InvoiceId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}