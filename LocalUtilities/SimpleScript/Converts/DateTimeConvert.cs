namespace LocalUtilities.SimpleScript.Converts;

internal sealed class DateTimeConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return DateTime.FromBinary(long.Parse(str));
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<DateTime>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is DateTime o)
            return o.ToBinary().ToString();
        throw TypeConvertException.CannotConvertToString<DateTime>(obj);
    }
}
