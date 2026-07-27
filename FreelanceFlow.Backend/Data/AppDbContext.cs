using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data;

/// <summary>
/// EF Core context. Inherits IdentityDbContext so ApplicationUser/roles get
/// the standard Identity tables (AspNetUsers, AspNetRoles, etc.) alongside
/// our own domain tables below.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractRiskAnalysis> ContractRiskAnalyses => Set<ContractRiskAnalysis>();
    public DbSet<Milestone> Milestones => Set<Milestone>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Picks up every IEntityTypeConfiguration<T> class in Data/Configurations
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}