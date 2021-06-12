using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum IndexType
    {
        PrimaryKey = 1,
        UniqueIndex = 2,
        UniqueConstraint = 3,
        NonUnique = 4
    }

    public partial class Index
    {
        public string Name { get; set; }
        public IndexType Type { get; set; }
        public IEnumerable<Column> Columns { get; set; }
        public bool IsClustered { get; set; }

        public bool IsUnique => Type != IndexType.NonUnique;

        public class Column
        {
            public string Name { get; set; }
            public int Order { get; set; }
            public SortDirection SortDirection { get; set; }

            public override bool Equals(object obj)
            {
                var col = obj as Column;
                return (col != null) ? col.Name.Equals(this.Name) : false;
            }

            public override int GetHashCode() => Name.GetHashCode();
        }
    }
}
