using Dapper;
using SqlSchema.Library.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer
    {
        private async Task AddTableFunctions(IDbConnection connection, List<DbObject> results)
        {
            var functions = await connection.QueryAsync<TableFunction>(
                @"SELECT 
                    [o].[name] AS [Name],
	                SCHEMA_NAME([o].[schema_id]) AS [Schema],
	                [o].[object_id] AS [Id],
                    [m].[definition] AS [SqlDefinition]
                FROM 
                    [sys].[objects] [o]
                    INNER JOIN [sys].[sql_modules] [m] ON [o].[object_id]=[m].[object_id]
                WHERE
	                [o].[type_desc]='SQL_TABLE_VALUED_FUNCTION'");

            var args = await connection.QueryAsync<Argument>(
                @"SELECT 
					[p].[object_id] AS [ObjectId],
					[p].[name] AS [Name],	
					TYPE_NAME([p].[system_type_id]) AS [DataType],
					CASE
						WHEN TYPE_NAME([p].[system_type_id]) LIKE 'nvar%' AND [p].[max_length]>0 THEN ([p].[max_length]/2)
						WHEN TYPE_NAME([p].[system_type_id]) LIKE 'nvar%' AND [p].[max_length]=0 THEN -1
						ELSE [p].[max_length]
					END AS [MaxLength],
					[p].[precision] AS [Precision],
					[p].[scale] AS [Scale],
					[p].[default_value] AS [DefaultValue],
					[p].[parameter_id] AS [Position]
				FROM 
					[sys].[all_parameters] [p]");

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
					INNER JOIN [sys].[all_objects] [f] ON [col].[object_id]=[f].[object_id]
					LEFT JOIN [sys].[computed_columns] [calc] ON [col].[object_id]=[calc].[object_id] AND [col].[column_id]=[calc].[column_id]
				WHERE
					[f].[type_desc]='SQL_TABLE_VALUED_FUNCTION'");

            var argLookup = args.ToLookup(row => row.ObjectId);
            var columnLookup = columns.ToLookup(row => row.ObjectId);

            foreach (var f in functions)
            {
                f.Arguments = argLookup[f.Id].ToArray();
                f.Columns = columnLookup[f.Id].ToArray();
            }

            results.AddRange(functions);
        }

    }
}
