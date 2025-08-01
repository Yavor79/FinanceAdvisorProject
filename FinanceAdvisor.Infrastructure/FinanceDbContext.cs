using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure.EntityConfigurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace FinanceAdvisor.Infrastructure
{
    public class FinanceDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<CreditConsultationCycle> CreditConsultationCycles { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<WeeklyAvailability> WeeklyAvailabilities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Apply Fluent API Configurations
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplyConfiguration(new WeeklyAvailabilityConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
            modelBuilder.ApplyConfiguration(new CreditConsultationCycleConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationConfiguration());
            modelBuilder.ApplyConfiguration(new AdvisorConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());



            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        }
    }
}
