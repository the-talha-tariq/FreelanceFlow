using Microsoft.AspNetCore.Identity;
using FreelanceFlow.Backend.Models.Enums;

namespace FreelanceFlow.Backend.Models.Entities;

/// <summary>
/// Extends ASP.NET Core Identity's IdentityUser with the extra profile
/// fields FreelanceFlow needs. Email/PasswordHash already come from
/// IdentityUser, so they are not repeated here.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public CurrencyType Currency { get; set; } = CurrencyType.USD;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}