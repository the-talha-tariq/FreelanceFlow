using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Company).HasMaxLength(200);
        builder.Property(c => c.Country).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Notes).HasMaxLength(2000);

        builder.HasOne(c => c.Freelancer)
            .WithMany(u => u.Clients)
            .HasForeignKey(c => c.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft-deleted clients are excluded everywhere by default;
        // repositories that need to see them use IgnoreQueryFilters().
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.FreelancerId);
    }
}