using Dapper;
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
        public override async Task<IEnumerable<DbObject>> GetDbObjectsAsync(IDbConnection connection)
        {
            List<DbObject> results = new List<DbObject>();

            await AddTablesAsync(connection, results);
            await AddForeignKeysAsync(connection, results);
            await AddViewsAsync(connection, results);
            await AddTableFunctions(connection, results);
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
    }
}
