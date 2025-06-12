using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using LapStore.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace LapStore.BLL.DependencyInjections
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceDependencyInjection(this IServiceCollection services, WebApplicationBuilder Builder)
        {
            //Register Services
            // Create one instance for the same request
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ICategoryFileHandler, CategoryFileHandler>();
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Builder.Environment.WebRootPath, "uploads")));
            
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IAdminUserService, AdminUserService>();


            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserClaimService, UserClaimService>();

            return services;
        }
    }
}
