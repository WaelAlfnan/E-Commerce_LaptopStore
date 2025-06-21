using Store.DAL.Data.Entities;
using Store.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Store.BLL.DependencyInjections
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositoryDependencyInjection(this IServiceCollection services)
        {
            // Register repositories

            services.AddScoped<IGenericRepository<Address>, GenericRepository<Address>>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICartRepository, CartRepository>();


            return services;
        }
    }
}

