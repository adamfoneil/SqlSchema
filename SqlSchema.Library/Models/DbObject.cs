using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public enum DbObjectType
    {
        Table,
        ForeignKey,
        View,
        TableFunction,
        Procedure,
        ScalarFunction
    }

    public abstract class DbObject
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        public abstract DbObjectType Type { get; }

        /// <summary>
        /// Object may be used in FROM clause and has columns
        /// </summary>
        public abstract bool IsSelectable { get; }

        public Column[] Columns { get; set; }

        public override bool Equals(object obj)
        {
            var table = obj as Table;
            return (table != null) ? table.GetHashCode().Equals(this.GetHashCode()) : false;
        }

        public override int GetHashCode()
        {
            return ((Schema ?? string.Empty) + (Name ?? string.Empty)).ToLower().GetHashCode();
        }
    }
}
