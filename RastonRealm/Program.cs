using Microsoft.EntityFrameworkCore;
using RastonRealm.Pages;

namespace RastonRealm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            var services = builder.Services;
            var configuration = builder.Configuration;


            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages(); // Routes for pages
                endpoints.MapControllers(); // Routes for API controllers
            });


            app.Run();
        }
    }
}