using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class InvoiceLineItemConfiguration : IEntityTypeConfiguration<InvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
    {
        builder.HasKey(li => li.Id);

        builder.Property(li => li.Description).IsRequired().HasMaxLength(500);
        builder.Property(li => li.Quantity).HasColumnType("decimal(10,2)");
        builder.Property(li => li.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(li => li.Total).HasColumnType("decimal(18,2)");

        builder.HasIndex(li => li.InvoiceId);
    }
}