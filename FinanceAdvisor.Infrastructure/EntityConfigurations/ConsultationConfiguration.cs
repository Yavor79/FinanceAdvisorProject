using FinanceAdvisor.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.HasKey(e => e.ConsultationId);

        builder.Property(e => e.ScheduledDateTime)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.ConsultationType)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        //builder.HasOne(e => e.Client)
        //    .WithMany(c => c.Consultations)
        //    .HasForeignKey(e => e.ClientId)
        //    .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne<User>() // Shadow nav
            .WithMany()
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(e => e.Advisor)
            .WithMany(a => a.Consultations)
            .HasForeignKey(e => e.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //builder.HasQueryFilter(e => e.Advisor.IsDeleted == false);
    }
}
