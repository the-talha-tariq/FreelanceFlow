using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title).IsRequired().HasMaxLength(300);
        builder.Property(c => c.Description).HasMaxLength(4000);
        builder.Property(c => c.TotalValue).HasColumnType("decimal(18,2)");
        builder.Property(c => c.DocumentPath).HasMaxLength(500);

        builder.HasOne(c => c.Freelancer)
            .WithMany(u => u.Contracts)
            .HasForeignKey(c => c.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Milestones)
            .WithOne(m => m.Contract)
            .HasForeignKey(m => m.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.RiskAnalyses)
            .WithOne(r => r.Contract)
            .HasForeignKey(r => r.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.FreelancerId);
        builder.HasIndex(c => c.ClientId);
        builder.HasIndex(c => c.Status);
    }
}