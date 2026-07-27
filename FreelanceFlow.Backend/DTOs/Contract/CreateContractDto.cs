using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.DTOs.Contracts;

public class CreateContractDto
{
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal TotalValue { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.USD;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}