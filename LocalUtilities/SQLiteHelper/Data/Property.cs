using LocalUtilities.General;

namespace LocalUtilities.SQLiteHelper;

public sealed class Property(string name, object? value) : IRosterItem<string>
{
    public string Name { get; } = name;

    public object? Value { get; } = value;

    public string Signature => Name;
}
