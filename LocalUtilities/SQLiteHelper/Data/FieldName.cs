using System.Reflection;

namespace LocalUtilities.SQLiteHelper;

public sealed class FieldName(
    string name,
    string properyName,
    Type type,
    bool isPrimaryKey,
    bool isUnique
)
{
    public string Name { get; } = name;

    public string PropertyName { get; } = properyName;

    public Type Type { get; } = type;

    public bool IsPrimaryKey { get; set; } = isPrimaryKey;

    public bool IsUnique { get; set; } = isUnique;

    public PropertyInfo? Property { get; } = null;

    public FieldName(string name, PropertyInfo property, bool isPrimaryKey, bool isUnique)
        : this(name, property.Name, property.PropertyType, isPrimaryKey, isUnique)
    {
        Property = property;
    }
}
