using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.IRepositories;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IAdvisorRepository : IRepository<Advisor, Guid>
    {
        Task<Advisor> GetByIdAsync(Guid id, bool ignoreFilters = false);

        IQueryable<Advisor> GetAllAttached(bool overrideQueryFilter = false);
    }

} 
    
