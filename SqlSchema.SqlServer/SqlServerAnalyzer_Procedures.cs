using Dapper;
using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer
    {
        private async Task AddProceduresAsync(IDbConnection connection, List<DbObject> results)
        {
            var procs = await connection.QueryAsync<Procedure>(
                @"SELECT
                    SCHEMA_NAME([p].[schema_id]) AS [Schema],
                    [p].[name] AS [Name],
                    [p].[object_id] AS [Id],
                    [m].[definition] AS [SqlDefinition]
                FROM 
                    [sys].[procedures] [p]
                    INNER JOIN [sys].[sql_modules] [m] ON [p].[object_id]=[m].[object_id]");
        }
    }
}
