using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title).IsRequired().HasMaxLength(300);
        builder.Property(m => m.Description).HasMaxLength(2000);
        builder.Property(m => m.Amount).HasColumnType("decimal(18,2)");

        // Milestone -> Invoice is optional 1-to-1: a milestone may or may not
        // have spawned an invoice yet. Configured from the Invoice side
        // (see InvoiceConfiguration) since Invoice holds the FK.

        builder.HasIndex(m => m.ContractId);
        builder.HasIndex(m => m.Status);
    }
}