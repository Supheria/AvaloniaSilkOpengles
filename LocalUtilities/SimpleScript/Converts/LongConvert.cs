namespace LocalUtilities.SimpleScript.Converts;

internal sealed class LongConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return long.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<long>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is long o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<long>(obj);
    }
}
