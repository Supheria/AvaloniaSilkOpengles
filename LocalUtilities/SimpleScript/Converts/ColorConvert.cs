using Avalonia.Media;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class ColorConvert : TypeConvert
{
    public override object ToType(string str)
    {
        try
        {
            return Color.Parse(str);
        }
        catch
        {
            throw TypeConvertException.CannotConvertToType<Color>(str);
        }
    }

    public override string ToString(object? obj)
    {
        if (obj is Color o)
            return o.ToString();
        throw TypeConvertException.CannotConvertToString<Color>(obj);
    }
}
