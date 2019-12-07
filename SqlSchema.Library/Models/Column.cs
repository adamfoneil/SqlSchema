namespace SqlSchema.Library.Models
{
    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Collation { get; set; }
        public bool IsNullable { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public string Expression { get; set; }
        public int Scale { get; set; }
        public bool InPrimaryKey { get; set; }        
        public int Position { get; set; }
        public int ObjectId { get; set; }       
    }
}
