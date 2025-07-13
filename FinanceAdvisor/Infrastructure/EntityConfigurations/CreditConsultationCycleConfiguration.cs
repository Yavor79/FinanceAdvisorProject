using FinanceAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CreditConsultationCycleConfiguration : IEntityTypeConfiguration<CreditConsultationCycle>
{
    public void Configure(EntityTypeBuilder<CreditConsultationCycle> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreditType)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Advisor)
            .WithMany()
            .HasForeignKey(e => e.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Meetings)
            .WithOne()
            .HasForeignKey("CreditConsultationCycleId");
    }
}
