using Dapper;
using SqlSchema.Library.Models;
using SqlSchema.SqlServer.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer
    {
        private async Task AddForeignKeysAsync(IDbConnection connection, List<DbObject> results)
        {
            var foreignKeys = await connection.QueryAsync<ForeignKeysResult>(
                @"SELECT
					[fk].[object_id] AS [ObjectId],
					[fk].[name] AS [ConstraintName],
					SCHEMA_NAME([ref_t].[schema_id]) AS [ReferencedSchema],
					[ref_t].[name] AS [ReferencedTable],
					SCHEMA_NAME([child_t].[schema_id]) AS [ReferencingSchema],
					[child_t].[name] AS [ReferencingTable],
					CONVERT(bit, [fk].[delete_referential_action]) AS [CascadeDelete],
					CONVERT(bit, [fk].[update_referential_action]) AS [CascadeUpdate]
				FROM
					[sys].[foreign_keys] [fk]
					INNER JOIN [sys].[tables] [ref_t] ON [fk].[referenced_object_id]=[ref_t].[object_id]
					INNER JOIN [sys].[tables] [child_t] ON [fk].[parent_object_id]=[child_t].[object_id]");

            var columns = await connection.QueryAsync<ForeignKeyColumnsResult>(
                @"SELECT
					[fkcol].[constraint_object_id] AS [ObjectId],
					[child_col].[name] AS [ReferencingName],
					[ref_col].[name] AS [ReferencedName]
				FROM
					[sys].[foreign_key_columns] [fkcol]
					INNER JOIN [sys].[tables] [child_t] ON [fkcol].[parent_object_id]=[child_t].[object_id]
					INNER JOIN [sys].[columns] [child_col] ON
						[child_t].[object_id]=[child_col].[object_id] AND
						[fkcol].[parent_column_id]=[child_col].[column_id]
					INNER JOIN [sys].[tables] [ref_t] ON [fkcol].[referenced_object_id]=[ref_t].[object_id]
					INNER JOIN [sys].[columns] [ref_col] ON
						[ref_t].[object_id]=[ref_col].[object_id] AND
						[fkcol].[referenced_column_id]=[ref_col].[column_id]");

            var colLookup = columns.ToLookup(row => row.ObjectId);

            results.AddRange(foreignKeys.Select(fk => new ForeignKey()
            {
                Name = fk.ConstraintName,
                ReferencedTable = new Table() { Schema = fk.ReferencedSchema, Name = fk.ReferencedTable },
                ReferencingTable = new Table() { Schema = fk.ReferencingSchema, Name = fk.ReferencingTable },
                CascadeDelete = fk.CascadeDelete,
                CascadeUpdate = fk.CascadeUpdate,
                Columns = colLookup[fk.ObjectId].Select(fkcol => new ForeignKeyColumn() { ReferencedName = fkcol.ReferencedName, ReferencingName = fkcol.ReferencingName }).ToArray()
            }));
        }
    }
}
