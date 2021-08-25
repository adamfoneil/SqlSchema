using SqlSchema.Library.Interfaces;
using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public class Procedure : DbObject, IDefinition
    {
        public override bool IsSelectable => false;

        public override DbObjectType Type => DbObjectType.Procedure;

        public string SqlDefinition { get; set; }

        public IEnumerable<Argument> Arguments { get; set; }
    }
}
