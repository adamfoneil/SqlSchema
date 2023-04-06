using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer.Extensions
{
    public static class ConnectionExtensions
    {
        public static async Task<bool> DatabaseExistsAsync(this IDbConnection connection, string databaseName) =>
            await connection.QuerySingleOrDefaultAsync<int>("SELECT 1 FROM [sys].[databases] WHERE [name]=@databaseName", new { databaseName }) == 1;
    }
}
