namespace SqlSchema.Library.Interfaces
{
    public interface IDataType
    {
        string DataType { get; }        
        int MaxLength { get; set; }
        int Precision { get; set; }
        int Scale { get; set; }
    }
}
