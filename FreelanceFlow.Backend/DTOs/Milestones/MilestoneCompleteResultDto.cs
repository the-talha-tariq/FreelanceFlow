namespace FreelanceFlow.Backend.DTOs.Milestones;

public class MilestoneCompleteResultDto
{
    public MilestoneDto Milestone { get; set; } = null!;
    public Guid GeneratedInvoiceId { get; set; }
    public string GeneratedInvoiceNumber { get; set; } = string.Empty;
}