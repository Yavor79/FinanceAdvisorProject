using Microsoft.EntityFrameworkCore;
//using FinanceAdvisor.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceAdvisor.Infrastructure.EntityConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
            .Property(e => e.Id)
            .IsRequired();

           

        }
    }
}
