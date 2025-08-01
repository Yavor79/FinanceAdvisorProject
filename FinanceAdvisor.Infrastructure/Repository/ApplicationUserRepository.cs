using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;


namespace FinanceAdvisor.Infrastructure.Repository
{
    public class ApplicationUserRepository : BaseRepository<ApplicationUser, Guid>, IApplicationUserRepository
    {
        public ApplicationUserRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
