
using ApiApplication.Model;

namespace ApiApplication.Endpoints;

public static class PeopleEndpoints
{
    public static void AddPeopleEndpoints(this WebApplication app)
    {
        app.MapGet("/people2", GetAllPeople);
        app.MapPost("/addperson", AddPerson);
    }

    private static async Task<IResult> GetAllPeople(SqlDataAccess sqlDataAccess)
    {
        List<PersonModel> people = [];
        try
        {
            people = (await sqlDataAccess.LoadData<PersonModel, dynamic>(
                "dbo.spPerson_GetAll", new { })).ToList();
        }
        catch (Exception ex)
        {
            return Results.Problem("An error occurred while retrieving data.");
        }
        return Results.Ok(people);

    }
    
    private static async Task<IResult> AddPerson(SqlDataAccess sqlDataAccess, PersonModel person)
    {
        var result = string.Empty;
        try
        {
            result = await sqlDataAccess.AddRow<PersonModel>(person);
        }
        catch (Exception ex)
        {
            return Results.Problem("An error occurred while retrieving data.");
        }
        return Results.Ok(result);

    }
}
