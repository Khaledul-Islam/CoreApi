namespace Utilities.Attributes
{
    public class ColumnNameAttribute(string columnName) : Attribute
    {
        public string ColumnName { get; } = columnName;
    }
}
