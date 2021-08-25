using SqlSchema.Library.Interfaces;
using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public class ScalarFunction : DbObject, IDefinition
    {
        public IEnumerable<Argument> Arguments { get; set; }

        public override bool IsSelectable => false;

        public override DbObjectType Type => DbObjectType.ScalarFunction;

        public string SqlDefinition => throw new System.NotImplementedException();
    }
}
