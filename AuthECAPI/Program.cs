using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Using Extension Method's

builder.Services
    .AddSwaggerExplorer() // Injecting the SwaggerAPI and SwaggerExplorer
    .InjectDbContext(builder.Configuration) // Included the DbContext DI
        .AddAppConfig(builder.Configuration) // Initializing the token variable to the Model for easy configuring secret token in API
    .AddIdentityHandlersAndStores() // Service from the Identity Core
    .ConfigureIdentityOption() // Customising the User Identity Validations
    .AddIdentityAuth(builder.Configuration); // Including the token - Authenticating and Authorization

var app = builder.Build();

// Using Extension Method's

app.ConfigureSwaggerExplorer() // Configure the HTTP request pipeline.
    .ConfigureCORS(builder.Configuration)   // Configure the CORS
    .AddIdentityAuthMiddlewares();  // Added the Middleware for Authentication and Authorization


app.UseHttpsRedirection();

app.MapControllers();

app
    .MapGroup("/api")   // Will group those api's with the prefix '/api'
    .MapIdentityApi<AppUser>();    // Mapped the identity API in the swagger

app
    .MapGroup("/api") // Mapping the prefix so that we don't need to specify each time
    .MapIdentityUserEndpoints(); // 



app.Run();

