using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;


namespace FinanceAdvisor.Infrastructure.Repository
{
    public class AdvisorRepository : BaseRepository<Advisor, Guid>, IAdvisorRepository
    {
        public AdvisorRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
