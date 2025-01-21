using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class PixelPointConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 2)
            return new PixelPoint(int.Parse(array[0]), int.Parse(array[1]));
        throw TypeConvertException.CannotConvertToType<PixelPoint>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is PixelPoint o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.X, o.Y);
        throw TypeConvertException.CannotConvertToString<PixelPoint>(obj);
    }
}
