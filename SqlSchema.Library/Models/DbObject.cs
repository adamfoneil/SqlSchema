using System.Collections.Generic;

namespace SqlSchema.Library.Models
{
    public enum DbObjectType
    {
        Table,
        View,
        TableFunction,
        Procedure,
        ScalarFunction
    }

    public abstract class DbObject
    {
        public string Schema { get; set; }
        public string Name { get; set; }

        public abstract DbObjectType Type { get; }

        public IEnumerable<Column> Columns { get; set; }

        /// <summary>
        /// Object may be used in FROM clause
        /// </summary>
        public abstract bool IsSelectable { get; }

        
    }
}
