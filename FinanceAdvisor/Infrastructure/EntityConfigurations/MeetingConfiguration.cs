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

        builder.HasOne(e => e.Client)
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Advisor)
            .WithMany(a => a.Meetings)
            .HasForeignKey(e => e.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
