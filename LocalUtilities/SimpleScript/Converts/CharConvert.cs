namespace LocalUtilities.SimpleScript.Converts;

internal sealed class CharConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return char.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<char>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is char o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<char>(obj);
    }
}
