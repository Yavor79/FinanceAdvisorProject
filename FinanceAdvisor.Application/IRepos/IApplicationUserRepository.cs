using FinanceAdvisor.Domain.Entities;
using System.Linq.Expressions;

namespace FinanceAdvisor.Application.IRepos
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetByIdsAsync(IEnumerable<Guid> userIds);
        //Task<IEnumerable<ApplicationUser>> GetFilteredAsync(Expression<Func<User, bool>> predicate);
        Task<bool> AddAsync(ApplicationUser user);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(Guid id);
    }
}
