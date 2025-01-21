namespace LocalUtilities.SQLiteHelper;

public sealed class FieldValue(string name, object? value, bool isPrimaryKey)
{
    public string Name { get; } = name;

    public object? Value { get; set; } = value;

    public bool IsPrimaryKey { get; set; } = isPrimaryKey;
}
