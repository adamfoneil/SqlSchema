using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public class TableFunction : DbObject
    {
        public IEnumerable<Argument> Arguments { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.TableFunction;
    }
}
