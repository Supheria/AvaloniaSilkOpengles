namespace LocalUtilities.SimpleScript.Converts;

internal sealed class ByteConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return byte.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<byte>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is byte o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<byte>(obj);
    }
}
