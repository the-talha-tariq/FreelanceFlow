using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Milestones;

public class UpdateMilestoneDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Pending/InProgress/Overdue only. Completed can't be set here — use
    /// POST /complete, which also generates the invoice.
    /// </summary>
    public MilestoneStatus Status { get; set; }
}