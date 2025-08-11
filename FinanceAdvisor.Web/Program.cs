using FinanceAdvisor.Web.Mappings;

using Microsoft.IdentityModel.Tokens;
using FinanceAdvisor.Web.Helpers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using static System.Net.WebRequestMethods;
using FinanceAdvisor.Web.Middlewares;
using FinanceAdvisor.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenLocalhost(5076, o => o.UseHttps());
            //    options.ListenLocalhost(5075); // HTTP
            //});

            

            builder.Services.AddHttpContextAccessor(); // Needed to access tokens from context

            builder.Services.AddTransient<AccessTokenHandler>();
            
            builder.Services.AddTransient<TokenRefreshService>();

            builder.Services.AddHttpClient("FinanceAdvisorAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7206/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler(); // Important if you're adding a DelegatingHandler manually
            })
            .AddHttpMessageHandler<AccessTokenHandler>();


            builder.Services.AddHttpClient("IdentityServerAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7150");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler(); // Important if you're adding a DelegatingHandler manually
            })
            .AddHttpMessageHandler<AccessTokenHandler>();


            builder.Services.AddAutoMapper(typeof(AdvisorProfile).Assembly);

            builder.Services.AddScoped<ITokenRefreshService, TokenRefreshService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies", options => {
                    
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        var userPrincipal = context.Principal;

                        // Debug claims
                        Console.WriteLine("Before fix claims:");
                        foreach (var claim in userPrincipal.Claims)
                        {
                            Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                        }
                        Console.WriteLine("1****************************clamRome:");
                        // Check if role claim is missing or wrong
                        if (userPrincipal.HasClaim(c => c.Type == ClaimTypes.Role)) {
                            var r = userPrincipal.FindFirst(c => c.Type == ClaimTypes.Role);
                            Console.WriteLine("****************************cROLE:"+r); }
                        
                        // Try to find the "role" claim (lowercase) or your actual claim type
                        var roleClaims = userPrincipal.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
                        Console.WriteLine("2****************************clamRome:");
                        if (roleClaims.Any())
                        {
                            var identity = (ClaimsIdentity)userPrincipal.Identity;

                            // Remove any incorrectly mapped role claims (optional)
                            var oldRoleClaims = identity.FindAll(ClaimTypes.Role).ToList();
                            foreach (var oldClaim in oldRoleClaims)
                            {
                                identity.RemoveClaim(oldClaim);
                            }
                            Console.WriteLine("****************************clamRome:");
                            // Add correct role claims
                            foreach (var rc in roleClaims)
                            {
                                Console.WriteLine("clamRome:"+rc);
                                identity.AddClaim(new Claim(ClaimTypes.Role, rc.Value));
                            }
                        }
                    

                        // Optionally, create a new ClaimsPrincipal and replace it:
                        context.ReplacePrincipal(userPrincipal);
                        

                        await Task.CompletedTask;
                    };
                    

                    options.Events.OnSigningIn = context =>
                    {
                        var identity = (ClaimsIdentity)context.Principal.Identity;

                        // Map raw 'role' claims to ClaimTypes.Role
                        var roleClaims = identity.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
                        foreach (var claim in roleClaims)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, claim.Value));
                        }

                        Console.WriteLine("🔐 Mapped roles during sign-in:");
                        foreach (var claim in identity.FindAll(ClaimTypes.Role))
                        {
                            Console.WriteLine($"Mapped Role: {claim.Value}");
                        }

                        var principal = context.Principal;

                        Console.WriteLine("🔐 Cookie is being issued!");
                        Console.WriteLine("Claims in the Principal:");
                        foreach (var claim in principal.Claims)
                        {
                            Console.WriteLine($"  {claim.Type} = {claim.Value}");
                        }

                        // You can inspect properties if needed
                        foreach (var prop in context.Properties.Items)
                        {
                            Console.WriteLine($"Property: {prop.Key} = {prop.Value}");
                        }

                        //var user = context.Principal;
                        //var roles = identity.FindAll(ClaimTypes.Role).ToArray();
                        //if (roles[0].Value == "Admin")
                        //{
                        //    context.Response.Redirect("/Admin/Home/Index");
                        //}

                        return Task.CompletedTask;
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:7150";
                    options.ClientId = "mvc_client";
                    options.ClientSecret = "mvc_secret";
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    // screws up the  /connect/userinfo Token validation
                    options.GetClaimsFromUserInfoEndpoint = false;

                    options.Scope.Add("finance_api");
                    options.Scope.Add("offline_access");
                    
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("roles");
                    options.CallbackPath = "/signin-oidc"; // MUST be handled
                    

                    // Use a cookie for correlation data (state, nonce, etc.)
                    options.UsePkce = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    
                    options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.Events.OnTokenResponseReceived = context =>
                    {
                        var accessToken = context.TokenEndpointResponse?.AccessToken;
                        Console.WriteLine("🔑 Access Token:");
                        Console.WriteLine(accessToken);
                        return Task.CompletedTask;
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



            builder.Services.AddControllersWithViews((options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseExceptionHandler("/Home/Error");
            app.UseStatusCodePagesWithRedirects("Home/Error?statusCode={0}");


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.Use(async (context, next) =>
            //{
            //    await next(); // Let the request run

            //    if (context.Response.Headers.TryGetValue("Set-Cookie", out var setCookie))
            //    {
            //        Console.WriteLine("🍪 Cookie(s) issued in response:");
            //        foreach (var cookie in setCookie)
            //        {
            //            Console.WriteLine(cookie);
            //        }
            //    }
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<AdminRedirectMiddleware>();
            //app.UseMiddleware<AdvisorRedirectMiddleware>();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //app.MapControllerRoute(
            //    name: "Errors",
            //    pattern: "{controller=Home}/{action=Index}/{statusCode?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            //app.MapRazorPages();


            app.Run();
        }
    }
}
