using Microsoft.EntityFrameworkCore;
using FinanceAdvisor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceAdvisor.Infrastructure.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder
            .Property(e => e.Id)
            .IsRequired();

           

        }
    }
}
