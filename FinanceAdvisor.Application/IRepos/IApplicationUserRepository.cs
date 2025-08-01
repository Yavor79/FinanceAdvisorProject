using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.IRepositories;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser, Guid>
    {
    }
}
