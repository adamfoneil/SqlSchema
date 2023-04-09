namespace SqlSchema.Library.Models
{
    public enum JoinCardinality
    {
        NotSet,
        OneToOne,
        OneToMany
    }

    public class ForeignKey : DbObject
    {
        public override DbObjectType Type => DbObjectType.ForeignKey;

        public override bool IsSelectable => false;

        public Table ReferencedTable { get; set; }
        public Table ReferencingTable { get; set; }
        public bool CascadeDelete { get; set; }
        public bool CascadeUpdate { get; set; }
        public JoinCardinality Cardinality { get; set; }

        public new ForeignKeyColumn[] Columns { get; set; }
    }
}
