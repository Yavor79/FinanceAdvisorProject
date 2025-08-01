using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.IRepositories;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IAdvisorRepository : IRepository<Advisor, Guid>
    {
    }
}
