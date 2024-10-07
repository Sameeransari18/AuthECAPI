using AuthECAPI.Models;
using System.Runtime.CompilerServices;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration config)
        {
            // Allowing CORS for frontend to utilise the APIs
            app.UseCors(options =>
            {
                options.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            return app;
        }
        
        public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration config)
        {
            // Initializing the token variable to the Model for easy configuring in API
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            return services;
        }
    }
}
