using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSchema.Library.Models
{
    public class Table : DbObject
    {
        public string IdentityColumn { get; set; }
        public string ClusteredIndex { get; set; }
        public long RowCount { get; set; }
        public string Alias { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.Table;

        public Index[] Indexes { get; set; }

        public HashSet<string> UniqueConstraintColumns
        {
            get
            {
                var results = Indexes
                    .Where(ndx => ndx.Type == IndexType.UniqueConstraint)
                    .SelectMany(ndx => ndx.Columns)
                    .Select(col => col.Name);

                return new HashSet<string>(results);
            }
        }

        public IEnumerable<ForeignKey> GetParentForeignKeys(IEnumerable<DbObject> allObjects)
        {
            return allObjects
                .OfType<ForeignKey>()
                .Where(fk => fk.ReferencingTable.Equals(this));
        }

        public IEnumerable<ForeignKey> GetChildForeignKeys(IEnumerable<DbObject> allObjects)
        {
            return allObjects
                .OfType<ForeignKey>()
                .Where(fk => fk.ReferencedTable.Equals(this));
        }

        public void EnumChildForeignKeys(IEnumerable<DbObject> allObjects, Action<Stack<ForeignKey>> starting = null, Action<Stack<ForeignKey>> ending = null)
        {
            Stack<ForeignKey> lineage = new Stack<ForeignKey>();

            Execute(this);

            void Execute(Table table)
            {
                var childFKs = table.GetChildForeignKeys(allObjects);
                foreach (var fk in childFKs)
                {
                    lineage.Push(fk);
                    starting?.Invoke(lineage);

                    Execute(fk.ReferencingTable);

                    ending?.Invoke(lineage);
                    lineage.Pop();
                }
            }
        }
    }
}
