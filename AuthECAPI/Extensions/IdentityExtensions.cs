using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace AuthECAPI.Extensions
{
    // Implemented the "Extension Method" as it requires class and method should be Static
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            // Moved the 
            services
                    .AddIdentityApiEndpoints<AppUser>()    // Used for the Endpoints
                    .AddEntityFrameworkStores<AppDbContext>();  // Used for the Db tables
            // Used for the Db tables

            return services;    // Returning the Service
        }

        public static IServiceCollection ConfigureIdentityOption(this IServiceCollection services)
        {
            // Customising the User Identity Validations
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = false;
            });

            return services;    // Returning the Service
        }

        // Auth = Authentication + Authorization
        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
        {

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                x.DefaultChallengeScheme =
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(y =>
            {
                y.SaveToken = false;    // Indicating we don't want to save the token once it's created.

                y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!))
                };      // Indicating to validate the token along with the JWT Secret Token.
            });

            return services;    // Returning the Service
        }

        // Included the Middleware - Authentication + Authorization
        public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();


            return app;    // Returning the Service
        }
    }
}
