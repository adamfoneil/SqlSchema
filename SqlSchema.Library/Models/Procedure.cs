namespace SqlSchema.Library.Models
{
    public class Procedure : DbObject
    {
        public string Sql { get; set; }

        public override bool IsSelectable => false;

        public override DbObjectType Type => DbObjectType.Procedure;
    }
}
