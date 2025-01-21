namespace LocalUtilities.SQLiteHelper;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TableField : Attribute
{
    public string? Name { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsUnique { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class TableIgnore : Attribute;
