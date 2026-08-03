namespace FreelanceFlow.Backend.DTOs.Milestones;

public class CreateMilestoneDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}