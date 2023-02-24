namespace SqlSchema.Library.Models
{
    public class Synonym : DbObject
    {
        public override DbObjectType Type => DbObjectType.Synonym;

        public override bool IsSelectable => false;

        public DbObject ReferencedObject { get; set; }
    }
}
