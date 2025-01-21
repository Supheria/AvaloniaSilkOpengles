namespace LocalUtilities.SimpleScript.Converts;

internal abstract class TypeConvert
{
    public abstract object ToType(string str);
    public abstract string ToString(object? obj);
}
