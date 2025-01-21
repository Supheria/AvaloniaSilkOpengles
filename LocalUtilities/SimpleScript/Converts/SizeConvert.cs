using Avalonia;
using LocalUtilities.SimpleScript.Common;

namespace LocalUtilities.SimpleScript.Converts;

internal sealed class SizeConvert : TypeConvert
{
    public override object ToType(string str)
    {
        var array = ArrayString.ToArray(SignTable.Seperator, str);
        if (array.Length is 2)
            return new Size(double.Parse(array[0]), int.Parse(array[1]));
        throw TypeConvertException.CannotConvertToType<Size>(str);
    }

    public override string ToString(object? obj)
    {
        if (obj is Size o)
            return ArrayString.ToArrayString(SignTable.Seperator, o.Width, o.Height);
        throw TypeConvertException.CannotConvertToString<Size>(obj);
    }
}
