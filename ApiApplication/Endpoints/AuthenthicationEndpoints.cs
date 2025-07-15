using ApiApplication.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiApplication.Endpoints
{
    public static class AuthenthicationEndpoints
    {
        public static void AddAuthenticationEndpoints(this WebApplication app)
        {
            app.MapPost("/signup", Signup);
            app.MapPost("/signin", Signin);
        }

        private static IResult Signup(UserLoginModel login, SqlDataAccess sqlDataAccess)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.Password);

            // Hasn't implemented
            Console.WriteLine(hashedPassword);
            return Results.Created();
        }

        private static async Task<IResult> Signin(UserLoginModel login, SqlDataAccess sqlDataAccess, IConfiguration configuration)
        {
            var user = await sqlDataAccess.GetUserByUserNameAsync<UserLoginModel>(login);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return Results.Unauthorized();
            }

            var claims = new[]
            {
                 new Claim(ClaimTypes.Name, login.Username)
            };

            var jwtKey = configuration["Jwt:Key"];
            var jwtIssuer = configuration["Jwt:Issuer"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

             return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });

            }
    }
}
