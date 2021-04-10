using SqlSchema.Library.Interfaces;

namespace SqlSchema.Library.Models
{
    public class View : DbObject, IDefinition
    {
        public string SqlDefinition { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.View;
    }
}
