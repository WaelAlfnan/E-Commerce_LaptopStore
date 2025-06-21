using Store.BLL.Interfaces;
using Store.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Store.BLL.DependencyInjections
{
    public static class PaymobDependencyInjection
    {
        public static IServiceCollection AddPaymobServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IPaymobService, PaymobService>(client =>
            {
                client.BaseAddress = new Uri(configuration["Paymob:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
} 
