using FinanceAdvisor.Domain.Entities;
//using FinanceAdvisor.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AdvisorConfiguration : IEntityTypeConfiguration<Advisor>
{
    public void Configure(EntityTypeBuilder<Advisor> builder)
    {
        builder.HasKey(e => e.AdvisorId);

        builder.Property(e => e.Specialization)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Advisor>(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(m => new { m.UserId })
            .IsUnique();

        builder
            .HasQueryFilter(m => m.IsDeleted == false);

        
    }
}
