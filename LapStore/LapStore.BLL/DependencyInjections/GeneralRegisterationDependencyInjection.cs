using Microsoft.Extensions.DependencyInjection;

namespace LapStore.BLL.DependencyInjections
{
    public static class GeneralRegisterationDependencyInjection
    {
        public static IServiceCollection AddGeneralDependencyInjection(this IServiceCollection services)
        {
            
            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            // Add CORS (allow any origin for development)
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            return services;
        }
    }
}
