namespace SqlSchema.Library.Models
{
    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool InPrimaryKey { get; set; }        
        public bool IsNullable { get; set; }
        public int Position { get; set; }
    }
}
