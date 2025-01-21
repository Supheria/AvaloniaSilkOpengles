using System.Reflection;
using Avalonia;
using Avalonia.Media;
using LocalUtilities.SimpleScript.Converts;

namespace LocalUtilities.SimpleScript;

public sealed partial class SerializeTool
{
    static BindingFlags Authority { get; } =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    static byte[] Utf8Bom { get; } = [0xEF, 0xBB, 0xBF];
    static Dictionary<Type, TypeConvert> TypeConverts { get; } =
        new()
        {
            [typeof(string)] = new StringConvert(),
            [typeof(bool)] = new BoolConvert(),
            [typeof(char)] = new CharConvert(),
            [typeof(byte)] = new ByteConvert(),
            [typeof(short)] = new ShortConvert(),
            [typeof(int)] = new IntConvert(),
            [typeof(long)] = new LongConvert(),
            [typeof(float)] = new FloatConvert(),
            [typeof(double)] = new DoubleConvert(),
            [typeof(Color)] = new ColorConvert(),
            [typeof(DateTime)] = new DateTimeConvert(),
            [typeof(PixelPoint)] = new PixelPointConvert(),
            [typeof(PixelSize)] = new PixelSizeConvert(),
            [typeof(PixelRect)] = new PixelRectConvert(),
            [typeof(Point)] = new PointConvert(),
            [typeof(Size)] = new SizeConvert(),
            [typeof(Rect)] = new RectConvert(),
        };
}
