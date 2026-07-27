using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.Models.Entities;

public class Contract
{
    public Guid Id { get; set; }

    public Guid FreelancerId { get; set; }
    public ApplicationUser Freelancer { get; set; } = null!;

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalValue { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Draft;

    /// <summary>Path to the uploaded contract document (PDF or text) on disk.</summary>
    public string? DocumentPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
    public ICollection<ContractRiskAnalysis> RiskAnalyses { get; set; } = new List<ContractRiskAnalysis>();
}