//using FinanceAdvisor.Domain.Entities;
//using FinanceAdvisor.Domain.Enums;
//using FinanceAdvisor.Infrastructure.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace FinanceAdvisor.Infrastructure.Seed
//{
//    public class DbSeeder
//    {
//        private readonly RoleManager<IdentityRole> roleManager;
//        private readonly UserManager<ApplicationUser> userManager;
//        private readonly IUserStore<ApplicationUser> userStore;
//        private readonly IUserEmailStore<ApplicationUser> emailStore;
//        private readonly IConfiguration configuration;

//        public static string[] roles = { "Admin", "Manager", "User" };
//        public DbSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
//            IUserStore<ApplicationUser> userStore, IConfiguration configuration)
//        {
//            this.roleManager = roleManager;
//            this.userManager = userManager;
//            this.userStore = userStore;
//            this.configuration = configuration;

//            //this.emailStore = this.GetEmailStore();
//        }
//        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
//        {
//            using var scope = serviceProvider.CreateScope();

//            var context = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//            // Apply pending migrations
//            await context.Database.MigrateAsync();

//            // Seed a user if none exist
//            if (!userManager.Users.Any())
//            {
//                var user = new ApplicationUser
//                {
//                    UserName = "advisor@example.com",
//                    Email = "advisor@example.com",
//                    EmailConfirmed = true
//                };

//                await userManager.CreateAsync(user, "P@ssword1!");
//            }

//            // Get created user
//            var advisorUser = await userManager.FindByEmailAsync("advisor@example.com");

//            // Seed Advisors
//            if (!context.Advisors.Any())
//            {
//                var advisor = new Advisor
//                {
//                    AdvisorId = Guid.NewGuid(),
//                    UserId = advisorUser.Id,
//                    CreatedAt = DateTime.UtcNow,
//                    Specialization = Specialization.Credit
//                };

//                context.Advisors.Add(advisor);
//                await context.SaveChangesAsync();
//            }

//            //// Seed Weekly Availability
//            //if (!context.WeeklyAvailabilities.Any())
//            //{
//            //    var advisor = await context.Advisors.FirstAsync();
//            //    context.WeeklyAvailabilities.AddRange(
//            //        new WeeklyAvailability
//            //        {
//            //            WeeklyAvailabilityId = Guid.NewGuid(),
//            //            AdvisorId = advisor.AdvisorId,
//            //            DayOfWeek = DayOfWeek.Monday,
//            //            StartTime = new TimeSpan(9, 0, 0),
//            //            EndTime = new TimeSpan(17, 0, 0)
//            //        }
//            //    );

//            //    await context.SaveChangesAsync();
//            //}

//            // Seed other entities as needed (Consultation, CreditCycle, etc.)
//        }

//        private async Task SeedRolesAsync()
//        {

//            foreach(var role in roles)
//            {
//                var roleExists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();

//                //if (!roleExists)
//                //{
//                //    //var result = roleManager.CreateAsync(new IdentityRole<Guid> { Name = role }).GetAwaiter().GetResult();
//                //    if (!result.Succeeded)
//                //    {
//                //        throw new Exception($"Failed to create role: {role}");
//                //    }
//                //}

                
//            }

//        }
//    }
//}
