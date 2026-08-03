using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(i => i.SubTotal).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TaxPercent).HasColumnType("decimal(5,2)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Notes).HasMaxLength(2000);
        builder.Property(i => i.PdfPath).HasMaxLength(500);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        builder.HasOne(i => i.Freelancer)
            .WithMany(u => u.Invoices)
            .HasForeignKey(i => i.FreelancerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Client)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional 1-to-1: MilestoneId is nullable because manually created
        // invoices aren't tied to a milestone. Restrict (not SetNull) here
        // deliberately: SQL Server rejects multiple cascade paths to the
        // same table, and Invoices is already reachable via
        // Client -> Invoice (Cascade). Adding a second cascading path via
        // Client -> Contract -> Milestone -> Invoice throws
        // "may cause cycles or multiple cascade paths" at migration time.
        // A milestone that has already spawned an invoice shouldn't be
        // deleted anyway, so Restrict matches the intended behavior.
        builder.HasOne(i => i.Milestone)
            .WithOne(m => m.Invoice)
            .HasForeignKey<Invoice>(i => i.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.LineItems)
            .WithOne(li => li.Invoice)
            .HasForeignKey(li => li.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.FreelancerId);
        builder.HasIndex(i => i.ClientId);
        builder.HasIndex(i => i.Status);
    }
}