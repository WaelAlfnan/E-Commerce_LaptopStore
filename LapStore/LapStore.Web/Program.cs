using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using LapStore.DAL;
using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register DbContext
            builder.Services.AddDbContext<LapStoreDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register repositories
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Create one instance for the same request
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.Run();
        }
    }
}