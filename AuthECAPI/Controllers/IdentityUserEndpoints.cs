using Microsoft.IdentityModel.Tokens;
using AuthECAPI.Controllers;
using AuthECAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthECAPI.Controllers
{
    public static class IdentityUserEndpoints
    {
        public class UserRegistrationModel
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {

            // Minimal API

            // Sign-up API
            app.MapPost("/signup", CreateUser); // Calling upon private method

            // Sign-in API
            app.MapPost("/signin", SignIn); // Calling upon private method

            return app;    // Returning the Service
        }

        private static async Task<IResult> CreateUser(
                UserManager<AppUser> userManager,
                [FromBody] UserRegistrationModel userRegistrationModel)
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
        }

        private static async Task<IResult> SignIn(
                UserManager<AppUser> userManager,
                [FromBody] LoginModel loginModel,
                IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);

            /**
                STRUCTURE OF JWT - Json Web Token 
                => base64UrlEncode(Header).base64UrlEncode(payload).Signature

                1) Header       - Algorithm and it's type
                2) Payload      - User's payload (such as UserId, Email, etc,.)
                3) Signature    - Encrypt's the token with the given secret token key 
             */

            if (user is not null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                // it is the security token
                var signInKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));


                // It is to determine what should be inside the token
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Indicates what else should be in Token
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("UserId", user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),   // Included the expiration of Token

                    SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token = token });
            }
            else
                return Results.BadRequest(new { Message = "Username or password is incorrect." });
        }
    }
}
