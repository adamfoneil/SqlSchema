﻿using Dapper;
using SqlSchema.Library.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            var args = await GetArgumentsAsync(connection);

            var argLookup = args.ToLookup(row => row.ObjectId);

            foreach (var proc in procs)
            {
                proc.Arguments = argLookup[proc.Id];
            }

            results.AddRange(procs);
        }
    }
}
