﻿using Dapper;
using SqlSchema.Library;
using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public class SqlServerAnalyzer : Analyzer
    {
        public override async Task<IEnumerable<DbObject>> GetDbObjectsAsync(IDbConnection connection)
        {
            List<DbObject> results = new List<DbObject>();

            await AddTablesAsync(connection, results);
            //await AddViewsAsync(connection, results);
            //await AddTableFunctions(connection, results);
            //await AddScalarFunctions(connection, results);
            //await AddProcedures(connection, results);

            return results;
        }

        private Task AddProcedures(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }

        private Task AddScalarFunctions(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }

        private Task AddTableFunctions(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }

        private Task AddViewsAsync(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }

        private async Task AddTablesAsync(IDbConnection connection, List<DbObject> results)
        {
            var tables = await connection.QueryAsync<Table>(
                @"WITH [clusteredIndexes] AS (
					SELECT [name], [object_id] FROM [sys].[indexes] WHERE [type_desc]='CLUSTERED'
				), [identityColumns] AS (
					SELECT [object_id], [name] FROM [sys].[columns] WHERE [is_identity]=1
				) SELECT
					[t].[name] AS [Name],
					SCHEMA_NAME([t].[schema_id]) AS [Schema],
					[t].[object_id] AS [Id],
					[c].[name] AS [ClusteredIndex],
					[i].[name] AS [IdentityColumn],
					(SELECT SUM(row_count) FROM [sys].[dm_db_partition_stats] WHERE [object_id]=[t].[object_id] AND [index_id] IN (0, 1)) AS [RowCount]
				FROM
					[sys].[tables] [t]
					LEFT JOIN [clusteredIndexes] [c] ON [t].[object_id]=[c].[object_id]
					LEFT JOIN [identityColumns] [i] ON [t].[object_id]=[i].[object_id]");

            var columns = await connection.QueryAsync<Column>(
                @"WITH [pkColumns] AS (
	                SELECT
		                [xcol].[object_id],		
		                [col].[name],
		                [col].[column_id]
	                FROM
		                [sys].[index_columns] [xcol]
		                INNER JOIN [sys].[indexes] [x] ON [xcol].[object_id]=[x].[object_id] AND [xcol].[index_id]=[x].[index_id]
		                INNER JOIN [sys].[columns] [col] ON [xcol].[object_id]=[col].[object_id] AND [xcol].[column_id]=[col].[column_id]
		                INNER JOIN [sys].[tables] [t] ON [x].[object_id]=[t].[object_id]
	                WHERE
		                [t].[type_desc]='USER_TABLE' AND
		                [x].[is_primary_key]=1
                ) SELECT
	                [col].[object_id] AS [ObjectId],
	                [col].[name] AS [Name],
	                TYPE_NAME([col].[system_type_id]) AS [DataType],
	                [col].[is_nullable] AS [IsNullable],
	                [def].[definition]  AS [DefaultValue],
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
	                CASE
		                WHEN [pk].[name] IS NOT NULL THEN 1
		                ELSE 0
	                END AS [InPrimaryKey]
                FROM
	                [sys].[columns] [col]
	                INNER JOIN [sys].[tables] [t] ON [col].[object_id]=[t].[object_id]
	                LEFT JOIN [sys].[default_constraints] [def] ON [col].[default_object_id]=[def].[object_id]
	                LEFT JOIN [sys].[computed_columns] [calc] ON [col].[object_id]=[calc].[object_id] AND [col].[column_id]=[calc].[column_id]
	                LEFT JOIN [pkColumns] [pk] ON [col].[object_id]=[pk].[object_id] AND [col].[column_id]=[pk].[column_id]
                WHERE
	                [t].[type_desc]='USER_TABLE'");

            var columnLookup = columns.ToLookup(row => row.ObjectId);

            foreach (var tbl in tables) tbl.Columns = columnLookup[tbl.Id].ToArray();

            results.AddRange(tables);
        }
    }
}