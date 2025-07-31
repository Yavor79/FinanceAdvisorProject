using FinanceAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WeeklyAvailabilityConfiguration : IEntityTypeConfiguration<WeeklyAvailability>
{
    public void Configure(EntityTypeBuilder<WeeklyAvailability> builder)
    {
        builder.HasKey(e => e.AvailabilityId);

        builder.Property(e => e.Weekday)
            .IsRequired();

        builder.Property(e => e.StartTime)
            .IsRequired();

        builder.Property(e => e.Duration)
            .IsRequired();
    }
}
