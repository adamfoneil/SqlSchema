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

        public IEnumerable<ForeignKey> GetForeignKeys(IEnumerable<DbObject> allObjects)
        {
            return allObjects
                .Where(obj => obj.Type == DbObjectType.ForeignKey && (obj as ForeignKey).ReferencingTable.Equals(this))
                .Select(obj => obj as ForeignKey);
        }
    }
}
