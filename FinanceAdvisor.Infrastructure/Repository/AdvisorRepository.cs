using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;


namespace FinanceAdvisor.Infrastructure.Repository
{
    public class AdvisorRepository : BaseRepository<Advisor, Guid>, IAdvisorRepository
    {
        public AdvisorRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<Advisor> GetAllAttached(bool overrideQueryFilter = false)
        {
            if (overrideQueryFilter)
                return dbSet.IgnoreQueryFilters().AsQueryable();
            else
                return dbSet.AsQueryable();
        }


        public async Task<Advisor> GetByIdAsync(Guid id, bool ignoreFilters = false)
        {
            if (ignoreFilters)
            {
                // Must use FirstOrDefaultAsync here — FindAsync does not work with IgnoreQueryFilters
                return await dbSet
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(a => a.AdvisorId == id);
            }
            else
            {
                // Still use query instead of FindAsync to ensure consistent behavior with predicates
                return await dbSet
                    .FirstOrDefaultAsync(a => a.AdvisorId == id);
            }
        }

    }
}
