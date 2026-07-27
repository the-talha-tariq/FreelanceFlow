using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Contracts;

/// <summary>
/// Returned by GET /api/contracts/{id}. Milestone and risk-analysis rows
/// themselves are fetched via their own endpoints
/// (GET /api/contracts/{id}/milestones, GET /api/contracts/{id}/analysis) —
/// this just gives enough of a summary to render a contract detail page.
/// </summary>
public class ContractDetailDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalValue { get; set; }
    public CurrencyType Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractStatus Status { get; set; }
    public string? DocumentPath { get; set; }
    public int MilestoneCount { get; set; }
    public int RiskFlagCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}