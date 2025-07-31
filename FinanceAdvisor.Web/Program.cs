using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace FinanceAdvisor.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.ListenLocalhost(5076, o => o.UseHttps());
            //    options.ListenLocalhost(5075); // HTTP
            //});


            // Add services to the container.
            builder.Services.AddHttpClient("FinanceAdvisorAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7206/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
