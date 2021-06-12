using SqlSchema.Library.Interfaces;

namespace SqlSchema.Library.Models
{
    public class Argument : IDataType
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public int Position { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public int ObjectId { get; set; }
    }
}
