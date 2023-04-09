using SqlSchema.Library;
using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSchema.SqlServer
{
    public partial class SqlServerAnalyzer : Analyzer
    {
        public bool UseMultipleActiveResultSets { get; set; }

        public override async Task<IEnumerable<DbObject>> GetDbObjectsAsync(IDbConnection connection)
        {
            List<DbObject> results = new List<DbObject>();

            if (UseMultipleActiveResultSets)
            {
                List<Task> tasks = new List<Task>
                {
                    AddTablesAsync(connection, results),
                    AddForeignKeysAsync(connection, results),
                    AddViewsAsync(connection, results),
                    AddTableFunctionsAsync(connection, results),
                    AddProceduresAsync(connection, results),
                    AddSynonymsAsync(connection, results)
                };

                await Task.WhenAll(tasks);
            }
            else
            {
                await AddTablesAsync(connection, results);
                await AddForeignKeysAsync(connection, results);
                await AddViewsAsync(connection, results);
                await AddTableFunctionsAsync(connection, results);                
                await AddProceduresAsync(connection, results);
                await AddSynonymsAsync(connection, results);
            }

            SetJoinCardinality(results);
            
            return results;
        }

        private void SetJoinCardinality(List<DbObject> results)
        {
            var foreignKeys = results.OfType<ForeignKey>();
            var tablesByName = results.OfType<Table>().ToDictionary(item => TableUniqueName(item));

            foreach (var fk in foreignKeys)
            {
                var referencingTable = tablesByName[TableUniqueName(fk.ReferencingTable)];
                fk.Cardinality = (MatchesUniqueConstraint(referencingTable, fk.Columns)) ? JoinCardinality.OneToOne : JoinCardinality.OneToMany;
            }

            bool MatchesUniqueConstraint(Table table, ForeignKeyColumn[] columns) =>
                table.Indexes.Where(IsUnique).Any(ndx => HaveSameColumns(ndx, columns));

            bool HaveSameColumns(Index ndx, ForeignKeyColumn[] columns)
            {
                var indexColumns = ndx.Columns.Select(col => col.Name.ToLower()).OrderBy(col => col).ToArray();
                var referencedColumns = columns.Select(col => col.ReferencingName.ToLower()).OrderBy(col => col).ToArray();
                return indexColumns.SequenceEqual(referencedColumns);
            }

            string TableUniqueName(Table table) => $"{table.Schema}.{table.Name}";
        }        

        private bool IsUnique(Index arg) => arg.IsUnique;        

        private Task AddScalarFunctions(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }
    }
}
