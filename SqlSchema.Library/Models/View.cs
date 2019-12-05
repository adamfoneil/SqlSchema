namespace SqlSchema.Library.Models
{
    public class View : DbObject
    {
        public string Sql { get; set; }

        public override bool IsSelectable => true;

        public override DbObjectType Type => DbObjectType.View;
    }
}
