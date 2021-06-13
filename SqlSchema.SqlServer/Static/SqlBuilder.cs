using SqlSchema.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SqlSchema.SqlServer.Static
{
    public static class SqlBuilder
    {
        public static string JoinTables(this IEnumerable<ForeignKey> foreignKeys)
        {
            // flatten all the tables shared by this FK list into a unique table list
            var tables = foreignKeys.SelectMany(fk => new Table[]
            {
                fk.ReferencedTable, // parent
                fk.ReferencingTable // child
            }).Distinct().ToArray();

            // make the FKs accessible by table name combo, both parent + child and child + parent
            var fkDictionary1 = foreignKeys.ToDictionary(fk => $"{fk.ReferencedTable}.{fk.ReferencingTable}");
            var fkDictionary2 = foreignKeys.ToDictionary(fk => $"{fk.ReferencingTable}.{fk.ReferencedTable}");

            var result = new StringBuilder();

            // first table is by itself
            result.AppendLine(TableName(tables[0]));

            // subsequent tables get join syntax
            for (int i = 1; i < tables.Length; i++)
            {
                var key = $"{tables[i]}.{tables[i - 1]}";
                // not working here because you can't assume that the table order is the join order
                var fk = (fkDictionary1.ContainsKey(key)) ? fkDictionary1[key] : fkDictionary2[key];

                var columns = string.Join(" AND ", fk.Columns.Select(col => $"{JoinAlias(fk.ReferencedTable)}.[{col.ReferencedName}] = {JoinAlias(fk.ReferencingTable)}.[{col.ReferencingName}]"));
                result.AppendLine($"INNER JOIN {TableName(tables[i])} ON {columns}");
            }

            return result.ToString();

            string TableName(Table table)
            {
                string name = AO.Models.Static.SqlBuilder.ApplyDelimiter(table.ToString(), '[', ']');
                return (string.IsNullOrEmpty(table.Alias)) ? name : $"{name} AS {JoinAlias(table)}";
            }

            string JoinAlias(Table table)
            {
                var name = !string.IsNullOrEmpty(table.Alias) ? table.Alias : table.Name;
                return AO.Models.Static.SqlBuilder.ApplyDelimiter(name, '[', ']');
            }
        }
    }
}
