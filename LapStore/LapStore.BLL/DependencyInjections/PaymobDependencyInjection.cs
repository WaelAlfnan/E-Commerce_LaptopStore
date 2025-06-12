using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LapStore.BLL.DependencyInjections
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