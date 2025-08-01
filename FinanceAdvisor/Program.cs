using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceAdvisor.Infrastructure;
using FinanceAdvisor.Infrastructure.Repository;
//using FinanceAdvisor.Infrastructure.Seed;
//using static FinanceAdvisor.Infrastructure.Seed.DbSeeder;
//using FinanceAdvisor.Infrastructure.Identity;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.Services;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore.Internal;
using FinanceAdvisor.Infrastructure.Seed.Seeders;



namespace FinanceAdvisor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenLocalhost(5001, o => o.UseHttps());
            //    options.ListenLocalhost(5000); // HTTP
            //});


            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
          

            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                ConfigureIdentity(builder, options);
            })
            .AddEntityFrameworkStores<FinanceDbContext>()
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins("https://localhost:7053;http://localhost:5024") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.RegisterRepositories(typeof(CreditConsultationCycleRepository).Assembly);
            builder.Services.RegisterServices(typeof(CreditConsultationCycleService).Assembly);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            AutoMapperConfig.Initialize();

            // TODO: Create create extension method for instancing all Seeders
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                //var appUserSeeder = new ApplicationUserSeeder();
                //await appUserSeeder.SeedAsync(services);

                //var advisorSeeder = new AdvisorSeeder();
                //await advisorSeeder.SeedAsync(services);

                //var consultationCycleSeeder = new CreditConsultationCycleSeeder();
                //await consultationCycleSeeder.SeedAsync(services);

                var consultationSeeder = new ConsultationSeeder();
                await consultationSeeder.SeedAsync(services);

                var meetingSeeder = new MeetingSeeder();
                await meetingSeeder.SeedAsync(services);
            }


            app.UseCors("AllowMVC");
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    await DbSeeder.SeedDataAsync(services);
            //}


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.MapControllers();

            app.Run();
        }

        private static void ConfigureIdentity(WebApplicationBuilder webAppBuilder, IdentityOptions identityOptions)
        {
            identityOptions.Password.RequireDigit =
                webAppBuilder.Configuration.GetValue<bool>("Identity:Password:RequireDigits");
            identityOptions.Password.RequireLowercase =
                webAppBuilder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
            identityOptions.Password.RequireUppercase =
                webAppBuilder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
            identityOptions.Password.RequireNonAlphanumeric =
                webAppBuilder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumerical");
            identityOptions.Password.RequiredLength =
                webAppBuilder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
            identityOptions.Password.RequiredUniqueChars =
                webAppBuilder.Configuration.GetValue<int>("Identity:Password:RequiredUniqueCharacters");

            identityOptions.SignIn.RequireConfirmedAccount =
                webAppBuilder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
            identityOptions.SignIn.RequireConfirmedEmail =
                webAppBuilder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedEmail");
            identityOptions.SignIn.RequireConfirmedPhoneNumber =
                webAppBuilder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedPhoneNumber");

            identityOptions.User.RequireUniqueEmail =
                webAppBuilder.Configuration.GetValue<bool>("Identity:User:RequireUniqueEmail");
        }
    }
}
