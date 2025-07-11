using ApiApplication;
using ApiApplication.Endpoints;
using ApiApplication.Model;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<SqlDataAccess>();

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

app.AddPeopleEndpoints(); // Registering the custom endpoint for people

app.MapGet("/people", async () =>
{
    List<PersonModel> people = new List<PersonModel>();
    try
    {
        var sqlDataAccess = app.Services.GetRequiredService<SqlDataAccess>();
        people = (await sqlDataAccess.LoadData<PersonModel, dynamic>(
            "dbo.spPerson_GetAll", new { })).ToList();
    }
    catch (Exception ex)
    {
        // Log the exception (not implemented here)
        return Results.Problem("An error occurred while retrieving data.");
    }
    return Results.Ok(people);
})
.WithName("GetPeople");


app.Run();
