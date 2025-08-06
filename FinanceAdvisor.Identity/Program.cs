
using Duende.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceAdvisor.Identity;
using FinanceAdvisor.Domain;

using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Infrastructure;
using Microsoft.AspNetCore.Identity.UI.Services;
using FinanceAdvisor.Infrastructure.Seed;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FinanceAdvisor.Identity
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFinanceApp", policy =>
                {
                    policy.WithOrigins("https://localhost:7053")
                          .AllowCredentials()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            // DB Context for ASP.NET Core Identity
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                ConfigureIdentity(builder, options);
            })
            .AddEntityFrameworkStores<FinanceDbContext>()
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>()
            .AddDefaultTokenProviders();
            // IdentityServer Configuration

            builder.Services.AddTransient<DefaultTokenCreationService>(); // Register the default one
            builder.Services.Replace(ServiceDescriptor.Transient<ITokenCreationService, LoggingTokenCreationService>()); // Replace ITokenCreationService with yours



            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                //options.EmitStaticAudienceClaim = true;
            })
                .AddAspNetIdentity<User>() // THIS tells IdentityServer : use ASP.NET Identity (with ApplicationUser) as your user store
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients)
                .AddDeveloperSigningCredential(); // Don't use in prod

            builder.Services.AddAuthentication()
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:7150"; // Your IdentityServer
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".AspNetCore.Identity.Application"; // Default value
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";

                Console.WriteLine($"🔐 Identity Cookie Name: {options.Cookie.Name}");

                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningIn = context =>
                    {
                        Console.WriteLine("🔐 Identity cookie being issued!");
                        foreach (var claim in context.Principal.Claims)
                        {
                            Console.WriteLine($"  {claim.Type}: {claim.Value}");
                        }

                        return Task.CompletedTask;
                    },
                    OnSignedIn = context =>
                    {
                        Console.WriteLine("✅ Cookie signed in.");
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                     policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
            });


            builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

            builder.Services.AddTransient<IProfileService, CustomProfileService>();

            builder.Services.AddTransient<RoleSeeder>();

            builder.Services.AddTransient<ITokenService, LoggingTokenService>();
            //builder.Services.AddSingleton<IEventSink, ConsoleLoggingEventSink>();
            


            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowFinanceApp");

            app.UseIdentityServer();  // IdentityServer BEFORE Authentication
            app.Use(async (context, next) =>
            {
                await next(); // Let the normal pipeline run

                if (context.Response.Headers.TryGetValue("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        if (cookie.StartsWith(".AspNetCore.Identity.Application"))
                        {
                            Console.WriteLine("🍪 Set-Cookie header:");
                            Console.WriteLine(cookie);
                        }
                    }
                }
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();
            app.MapGet("/is-authenticated", (HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    return Results.Json(new { authenticated = true, user = context.User.Identity.Name });
                }

                return Results.Json(new { authenticated = false });
            });

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;

            //    var roleSeeder = services.GetRequiredService<RoleSeeder>();
            //    //await roleSeeder.SeedRolesAsync();

            //    //await roleSeeder.SeedRolesToUsersAsync();

            //}

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
