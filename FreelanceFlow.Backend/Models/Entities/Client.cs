namespace FreelanceFlow.Backend.Models.Entities;

public class Client
{
    public Guid Id { get; set; }

    public Guid FreelancerId { get; set; }
    public ApplicationUser Freelancer { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string Country { get; set; } = string.Empty;
    public int PaymentTermsDays { get; set; } = 14;
    public string? Notes { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}