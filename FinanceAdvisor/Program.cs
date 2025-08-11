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
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanceAdvisor.Infrastructure.Seed;



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


            // no Identity here -> users log in the Identity Server !!!

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.WithOrigins("https://localhost:7053", "http://localhost:5024")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });



            builder.Services.AddAuthentication("Bearer")

            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "https://localhost:7150"; // IdentityServer
                options.Audience = "finance_api";
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine($"Authentication failed: {ctx.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        Console.WriteLine("Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true,

                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));

                options.AddPolicy("AdvisorOnly", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Advisor"));

                options.AddPolicy("UserOnly", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "User"));
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

                //var consultationSeeder = new ConsultationSeeder();
                //await consultationSeeder.SeedAsync(services);

                //var meetingSeeder = new MeetingSeeder();
                //await meetingSeeder.SeedAsync(services);
            }


            app.UseCors("AllowMVC");
            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


            app.Run();
        }

    }
}
