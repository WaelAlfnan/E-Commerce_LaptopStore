using LapStore.BLL.DependencyInjections;

namespace LapStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddServiceDependencyInjection(builder)
                            .AddRepositoryDependencyInjection()
                            .AddDbContextDependencyInjection(builder.Configuration)
                            .AddIdentityDependencyInjection(builder.Configuration)
                            .AddGeneralDependencyInjection();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAll");

            // Add detailed request logging
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
                logger.LogInformation($"Authorization Header: {context.Request.Headers["Authorization"]}");
                logger.LogInformation($"Content-Type: {context.Request.Headers["Content-Type"]}");
                logger.LogInformation($"Accept: {context.Request.Headers["Accept"]}");
                
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing request: {ex}");
                    throw;
                }
                
                logger.LogInformation($"Response Status Code: {context.Response.StatusCode}");
                logger.LogInformation($"Response Headers: {string.Join(", ", context.Response.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
