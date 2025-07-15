using ApiApplication.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ApiApplication;

public class SqlDataAccess
{
    private readonly IConfiguration _configuration;

    public SqlDataAccess(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName = "default")
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString(connectionStringName));
        
        var row  = await connection.QueryAsync<T>(
            storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);

        return row;
    }

    public async Task<string> AddRow<U>(U parameters, string connectionStringName = "default")
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString(connectionStringName));
        
        string sql = "INSERT INTO Person (FirstName, LastName) VALUES (@FirstName, @LastName)";

        var row = await connection.ExecuteAsync(sql, parameters);

        //var row = await connection.ExecuteAsync(
        //    storedProcedure, parameters, commandType: System.Data.CommandType.StoredProcedure);

        return "Success";
    }

    public async Task<UserLoginModel> GetUserByUserNameAsync<U>(U parameters, string connectionStringName = "default")
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString(connectionStringName));

        string sql = "SELECT Username, PasswordHash as Password FROM UserLogin WHERE Username = @Username";

        var row = await connection.QueryFirstOrDefaultAsync<UserLoginModel>(sql, parameters);

        return row;
    }
}
