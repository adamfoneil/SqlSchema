namespace SqlSchema.Library.Models
{
    public class Table : DbObject
    {
        public string IdentityColumn { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.Table;
    }
}
