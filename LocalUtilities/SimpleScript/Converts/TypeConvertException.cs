namespace LocalUtilities.SimpleScript.Converts;

internal class TypeConvertException(string message) : Exception(message)
{
    public static TypeConvertException CannotConvertToType<T>(string str)
    {
        return new($"cannot convert \"{str}\" to {typeof(T).FullName}");
    }

    public static TypeConvertException CannotConvertToString<T>(object? obj)
    {
        return new($"cannot convert {obj} to string of {typeof(T).FullName}");
    }
}
