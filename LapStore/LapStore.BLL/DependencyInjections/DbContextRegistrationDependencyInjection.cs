using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSConfiguration = Microsoft.Extensions.Configuration;

namespace LapStore.BLL.DependencyInjections
{
    public static class DBContextRegistrationDependencyInjection
    {
        public static IServiceCollection AddDbContextDependencyInjection(this IServiceCollection services, MSConfiguration.IConfiguration configuration)
        {

            // Register DbContext with existing database
            services.AddDbContext<LapStoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlServerOptions.CommandTimeout(30);
                        sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                        sqlServerOptions.ExecutionStrategy(dependencies => 
                            new SqlServerRetryingExecutionStrategy(
                                context: dependencies.CurrentContext.Context,
                                maxRetryCount: 5));
                    });
                
                // Enable diagnostic features
                options.EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            return services;
        }
    }
}
