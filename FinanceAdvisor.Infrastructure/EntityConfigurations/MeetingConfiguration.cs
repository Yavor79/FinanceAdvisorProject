using FinanceAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ScheduledDateTime)
            .IsRequired();

        builder.Property(e => e.Type)
            .IsRequired();

        builder
            .HasOne(e => e.CreditConsultationCycle)
            .WithMany(c => c.Meetings)
            .HasForeignKey(e => e.CreditConsultationCycleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);


    }
}
