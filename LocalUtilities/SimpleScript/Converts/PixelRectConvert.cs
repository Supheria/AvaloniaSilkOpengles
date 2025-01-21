using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class PixelRectConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 4)
            return new PixelRect(
                int.Parse(array[0]),
                int.Parse(array[1]),
                int.Parse(array[2]),
                int.Parse(array[3])
            );
        throw TypeConvertException.CannotConvertToType<PixelRect>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is PixelRect o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.X, o.Y, o.Width, o.Height);
        throw TypeConvertException.CannotConvertToString<PixelRect>(obj);
    }
}
