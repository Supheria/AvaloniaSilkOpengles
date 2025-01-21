using System.Globalization;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class DoubleConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return double.Parse(str, CultureInfo.InvariantCulture);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<double>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is double o)
            return o.ToString(CultureInfo.InvariantCulture);
        throw TypeConvertException.CannotConvertToString<double>(obj);
    }
}
