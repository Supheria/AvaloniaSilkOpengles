namespace LocalUtilities.SimpleScript.Converts;

internal class StringConvert : TypeConvert
{
    public override object ToType(string str)
    {
        return str;
    }

    public override string ToString(object? obj)
    {
        if (obj is string o)
            return o;
        throw TypeConvertException.CannotConvertToString<string>(obj);
    }
}
