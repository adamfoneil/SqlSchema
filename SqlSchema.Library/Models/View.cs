namespace SqlSchema.Library.Models
{
    public class View : DbObject
    {
        public string Definition { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.View;
    }
}
