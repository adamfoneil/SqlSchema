using SqlSchema.Library.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SqlSchema.Library
{
    public abstract class Analyzer
    {
        public abstract Task<IEnumerable<DbObject>> GetDbObjectsAsync(IDbConnection connection);
    }
}
