namespace SqlSchema.Library.Models
{
    public class ForeignKeyColumn
    {
        public string ReferencedName { get; set; }
        public string ReferencingName { get; set; }
    }
}
