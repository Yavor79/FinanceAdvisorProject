using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceAdvisor.Infrastructure.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly FinanceDbContext _dbContext;

        public ApplicationUserRepository(FinanceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user == null ? null : MapToDomain(user);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            var users = await _dbContext.Users.ToListAsync();
            return users.Select(MapToDomain);
        }

        //public async Task<IEnumerable<ApplicationUser>> GetFilteredAsync(Expression<Func<User, bool>> predicate)
        //{
        //    var users = await _dbContext.Users
        //        .Where(predicate)
        //        .ToListAsync();

        //    return users.Select(MapToDomain);
        //}


        public async Task<IEnumerable<ApplicationUser>> GetByIdsAsync(IEnumerable<Guid> userIds)
        {
            var users = await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            return users.Select(MapToDomain);
        }

        public async Task<bool> AddAsync(ApplicationUser user)
        {
            var entity = new User
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.Email,
                NormalizedEmail = user.Email?.ToUpper(),
                NormalizedUserName = user.Email?.ToUpper(),
                EmailConfirmed = true, // You can adjust based on your needs
                IsDeleted = user.IsDeleted
            };

            _dbContext.Users.Add(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            var entity = await _dbContext.Users.FindAsync(user.Id);
            if (entity == null) return false;

            entity.Email = user.Email;
            entity.UserName = user.Email;
            entity.NormalizedEmail = user.Email?.ToUpper();
            entity.NormalizedUserName = user.Email?.ToUpper();
            entity.IsDeleted = user.IsDeleted;

            _dbContext.Users.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Users.FindAsync(id);
            if (entity == null) return false;

            _dbContext.Users.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // Utility mapper
        private ApplicationUser MapToDomain(User user)
        {
            return new ApplicationUser
            {
                Id = user.Id,
                Email = user.Email,
                IsDeleted = user.IsDeleted
            };
        }
    }
}
