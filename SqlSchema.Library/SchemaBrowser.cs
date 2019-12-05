using SqlSchema.Library.Models;
using System.Collections.Generic;
using System.Data;

namespace SqlSchema.Library
{
    public abstract class SchemaBrowser
    {
        public abstract IEnumerable<DbObject> GetDbObjects(IDbConnection connection);
    }
}
