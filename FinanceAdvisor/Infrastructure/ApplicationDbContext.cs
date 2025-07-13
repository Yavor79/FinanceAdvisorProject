using FinanceAdvisor.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceAdvisor.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<CreditConsultationCycle> CreditConsultationCycles { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<WeeklyAvailability> WeeklyAvailabilities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Fluent API Configurations
            modelBuilder.ApplyConfiguration(new WeeklyAvailabilityConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
            modelBuilder.ApplyConfiguration(new CreditConsultationCycleConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationConfiguration());
            modelBuilder.ApplyConfiguration(new AdvisorConfiguration());

            // Add other configurations as you define them (User, Payment, Notification, etc.)
        }
    }
}
