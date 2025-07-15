using ApiApplication;
using ApiApplication.Endpoints;
using ApiApplication.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SqlDataAccess>();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Scalar API Reference";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.AddPeopleEndpoints();
app.AddAuthenticationEndpoints();

app.MapGet("/people", async (SqlDataAccess sqlDataAccess) =>
{
    List<PersonModel> people = new List<PersonModel>();
    try
    {
        //var sqlDataAccess = app.Services.GetRequiredService<SqlDataAccess>();
        people = (await sqlDataAccess.LoadData<PersonModel, dynamic>(
            "dbo.spPerson_GetAll", new { })).ToList();
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while retrieving data.");
    }
    return Results.Ok(people);
})
.WithName("GetPeople")
.RequireAuthorization();

app.Run();
