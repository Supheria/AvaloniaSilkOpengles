using System.Globalization;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class FloatConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<float>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is float o)
            return o.ToString(CultureInfo.InvariantCulture);
        throw TypeConvertException.CannotConvertToString<float>(obj);
    }
}
