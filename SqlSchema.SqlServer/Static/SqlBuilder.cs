using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;

namespace SqlSchema.SqlServer.Static
{
    public static class SqlBuilder
    {
        public static IEnumerable<string> FromClause(this IEnumerable<ForeignKey> foreignKeys, Dictionary<Table, string> aliases = null)
        {
            throw new NotImplementedException();
        }
    }
}
