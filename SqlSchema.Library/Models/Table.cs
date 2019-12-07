using System.Collections.Generic;
using System.Linq;

namespace SqlSchema.Library.Models
{
    public class Table : DbObject
    {
        public string IdentityColumn { get; set; }
        public string ClusteredIndex { get; set; }
        public long RowCount { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.Table;

        public IEnumerable<ForeignKey> GetParentForeignKeys(IEnumerable<DbObject> allObjects)
        {
            return allObjects
                .OfType<ForeignKey>()
                .Where(fk => fk.ReferencingTable.Equals(this))
                .Select(obj => obj as ForeignKey);
        }

        public IEnumerable<ForeignKey> GetChildForeignKeys(IEnumerable<DbObject> allObjects)
        {
            return allObjects
                .OfType<ForeignKey>()
                .Where(fk => fk.ReferencedTable.Equals(this))
                .Select(obj => obj as ForeignKey);
        }
    }
}
