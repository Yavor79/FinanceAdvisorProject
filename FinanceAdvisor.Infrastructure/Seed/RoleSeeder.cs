using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceAdvisor.Infrastructure.Seed
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IUserStore<User> userStore;
        private readonly IUserEmailStore<User> emailStore;
        private readonly IConfiguration configuration;

        public static string[] roles = { "Admin", "Advisor", "User" };
        public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, UserManager<User> userManager,
            IUserStore<User> userStore, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.userStore = userStore;
            this.configuration = configuration;

            //this.emailStore = this.GetEmailStore();
        }
        
        public async Task SeedRolesAsync()
        {

            foreach (var role in roles)
            {
                var roleExists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();

                if (!roleExists)
                {
                    var result = roleManager.CreateAsync(new IdentityRole<Guid>(role)).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }

            }

        }

        public async Task SeedRolesToUsersAsync()
        {

            var users = userManager.Users.ToList();
            int count = 0;
            for (int i = 0; i < 1; i++)
            {
                await userManager.AddToRoleAsync(users[count], "Admin");
                count++;
            }
            for (int i = 0; i < 2; i++)
            {
                await userManager.AddToRoleAsync(users[count], "Advisor");
                count++;
            }
            for (int i = 0; i < 3; i++)
            {
                await userManager.AddToRoleAsync(users[count], "User");
                count++;
            }
        }
    }
}
