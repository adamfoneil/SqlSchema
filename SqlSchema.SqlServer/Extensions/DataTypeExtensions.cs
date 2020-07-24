using SqlSchema.Library.Interfaces;

namespace SqlSchema.SqlServer.Extensions
{
    public static class DataTypeExtensions
    {
        public static string DisplayDataType(this IDataType dataType) =>
            (dataType.DataType.StartsWith("nvar") || dataType.DataType.StartsWith("var")) ?
            (dataType.MaxLength == -1) ? $"{dataType.DataType}(max)" : $"{dataType.DataType}({dataType.MaxLength})" :
            (dataType.DataType.Contains("decimal")) ? $"decimal({dataType.Scale}, {dataType.Precision})" :
            dataType.DataType;
    }
}
