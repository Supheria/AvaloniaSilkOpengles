using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class RectConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 4)
            return new Rect(
                double.Parse(array[0]),
                double.Parse(array[1]),
                double.Parse(array[2]),
                double.Parse(array[3])
            );
        throw TypeConvertException.CannotConvertToType<Rect>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is Rect o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.X, o.Y, o.Width, o.Height);
        throw TypeConvertException.CannotConvertToString<Rect>(obj);
    }
}
