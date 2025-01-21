using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class PointConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 2)
            return new Point(double.Parse(array[0]), double.Parse(array[1]));
        throw TypeConvertException.CannotConvertToType<Point>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is Point o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.X, o.Y);
        throw TypeConvertException.CannotConvertToString<Point>(obj);
    }
}
