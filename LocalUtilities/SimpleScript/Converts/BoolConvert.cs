namespace LocalUtilities.SimpleScript.Converts;

internal sealed class BoolConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return bool.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<bool>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is bool o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<bool>(obj);
    }
}
