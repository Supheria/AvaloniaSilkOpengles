namespace LocalUtilities.SimpleScript.Converts;

internal sealed class IntConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return int.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<int>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is int o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<int>(obj);
    }
}
