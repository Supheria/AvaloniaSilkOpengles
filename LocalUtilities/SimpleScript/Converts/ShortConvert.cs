namespace LocalUtilities.SimpleScript.Converts;

internal sealed class ShortConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return short.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<short>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is short o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<short>(obj);
    }
}
