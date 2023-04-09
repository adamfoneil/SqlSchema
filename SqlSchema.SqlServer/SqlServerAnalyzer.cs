using SqlSchema.Library;
using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
            
            return results;
        }

        private Task AddScalarFunctions(IDbConnection connection, List<DbObject> results)
        {
            throw new NotImplementedException();
        }
    }
}
