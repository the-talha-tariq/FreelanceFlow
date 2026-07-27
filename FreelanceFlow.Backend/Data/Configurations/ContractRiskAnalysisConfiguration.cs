using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FreelanceFlow.Backend.Models.Entities;

namespace FreelanceFlow.Backend.Data.Configurations;

public class ContractRiskAnalysisConfiguration : IEntityTypeConfiguration<ContractRiskAnalysis>
{
    public void Configure(EntityTypeBuilder<ContractRiskAnalysis> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RawAIResponse).HasColumnType("nvarchar(max)");
        builder.Property(r => r.ExtractedText).HasMaxLength(2000);
        builder.Property(r => r.Explanation).HasMaxLength(2000);
        builder.Property(r => r.SuggestedAlternative).HasMaxLength(2000);

        builder.HasIndex(r => r.ContractId);
    }
}