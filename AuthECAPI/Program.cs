using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service from the Identity Core
builder.Services
    .AddIdentityApiEndpoints<AppUser>()    // Used for the Endpoints
    .AddEntityFrameworkStores<AppDbContext>();  // Used for the Db tables

// Included the DbContext DI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDB")));

// Customising the User Identity Validations
builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = false;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Config. CORS
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader();
});
#endregion

app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("/api")   // Will group those api's with the prefix '/api'
    .MapIdentityApi<AppUser>();    // Mapped the identity API in the swagger


// Minimal API

app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel
    ) =>
    {
        AppUser user = new AppUser()
        {
            UserName = userRegistrationModel.Email,
            Email = userRegistrationModel.Email,
            FullName = userRegistrationModel.FullName,
        };

        var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

        if (result.Succeeded)
            return Results.Ok(result);
        else
            return Results.BadRequest(result);
    });


app.Run();

public class UserRegistrationModel()
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}