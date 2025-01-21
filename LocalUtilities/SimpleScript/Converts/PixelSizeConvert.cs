using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class PixelSizeConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 2)
            return new PixelSize(int.Parse(array[0]), int.Parse(array[1]));
        throw TypeConvertException.CannotConvertToType<PixelSize>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is PixelSize o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.Width, o.Height);
        throw TypeConvertException.CannotConvertToString<PixelSize>(obj);
    }
}
