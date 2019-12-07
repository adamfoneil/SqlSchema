namespace SqlSchema.Library.Models
{
    public class ForeignKey : DbObject
    {
        public override DbObjectType Type => DbObjectType.ForeignKey;

        public override bool IsSelectable => false;

        public Table ReferencedTable { get; set; }
        public Table ReferencingTable { get; set; }
        public bool CascadeDelete { get; set; }
        public bool CascadeUpdate { get; set; }

        public new ForeignKeyColumn[] Columns { get; set; }
    }
}
