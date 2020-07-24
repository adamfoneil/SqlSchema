using Dapper;
using SqlSchema.Library;
using SqlSchema.Library.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer : Analyzer
    {
        private async Task AddViewsAsync(IDbConnection connection, List<DbObject> results)
        {
            var views = await connection.QueryAsync<View>(
                @"SELECT 
                    [v].[name] AS [Name],
	                SCHEMA_NAME([v].[schema_id]) AS [Schema],
	                [v].[object_id] AS [Id],
                    [m].[definition] AS [Definition]
                FROM 
                    [sys].[views] [v]
                    INNER JOIN [sys].[sql_modules] [m] ON [v].[object_id]=[m].[object_id]");

			var columns = await connection.QueryAsync<Column>(
				@"SELECT
	                [col].[object_id] AS [ObjectId],
	                [col].[name] AS [Name],
	                TYPE_NAME([col].[system_type_id]) AS [DataType],
	                [col].[is_nullable] AS [IsNullable],	                
	                [col].[collation_name] AS [Collation],
	                CASE
		                WHEN TYPE_NAME([col].[system_type_id]) LIKE 'nvar%' AND [col].[max_length]>0 THEN ([col].[max_length]/2)
		                WHEN TYPE_NAME([col].[system_type_id]) LIKE 'nvar%' AND [col].[max_length]=0 THEN -1
		                ELSE [col].[max_length]
	                END AS [MaxLength],
	                [col].[precision] AS [Precision],
	                [col].[scale] AS [Scale],
	                [col].[column_id] AS [Position],
	                [calc].[definition] AS [Expression],
	                0 AS [InPrimaryKey]
                FROM
	                [sys].[columns] [col]	
					INNER JOIN [sys].[views] [v] ON [col].[object_id]=[v].[object_id]
	                LEFT JOIN [sys].[computed_columns] [calc] ON [col].[object_id]=[calc].[object_id] AND [col].[column_id]=[calc].[column_id]");

			var columnLookup = columns.ToLookup(row => row.ObjectId);

			foreach (var v in views) v.Columns = columnLookup[v.Id].ToArray();

			results.AddRange(views);
		}
	}
}
