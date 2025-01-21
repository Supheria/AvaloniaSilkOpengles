using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator.DLA;

internal sealed class DlaPixel
{
    internal int X { get; }

    internal int Y { get; }

    public DlaPixel(int x, int y)
    {
        X = x;
        Y = y;
    }

    internal Dictionary<Direction, PixelPoint> Neighbor { get; } = [];

    internal Dictionary<Direction, int> ConnetNumber { get; } = [];

    internal double Altitude
    {
        get
        {
            if (_altitude is -1)
            {
                if (!ConnetNumber.TryGetValue(Direction.Top, out var top))
                    top = 0;
                if (!ConnetNumber.TryGetValue(Direction.Bottom, out var bottom))
                    bottom = 0;
                if (!ConnetNumber.TryGetValue(Direction.Left, out var left))
                    left = 0;
                if (!ConnetNumber.TryGetValue(Direction.Right, out var right))
                    right = 0;
                if (!ConnetNumber.TryGetValue(Direction.LeftTop, out var leftTop))
                    leftTop = 0;
                if (!ConnetNumber.TryGetValue(Direction.BottomRight, out var bottomRight))
                    bottomRight = 0;
                if (!ConnetNumber.TryGetValue(Direction.TopRight, out var topRight))
                    topRight = 0;
                if (!ConnetNumber.TryGetValue(Direction.LeftBottom, out var leftBottom))
                    leftBottom = 0;
                var list = new List<int>(4)
                {
                    top + bottom - Math.Abs(top - bottom),
                    left + right - Math.Abs(left - right),
                    leftTop + bottomRight - Math.Abs(leftTop - bottomRight),
                    topRight + leftBottom - Math.Abs(topRight - leftBottom),
                };
                _altitude = list.Max();
            }
            return _altitude;
        }
        set => _altitude = value;
    }
    double _altitude = -1;

    internal DlaPixel()
        : this(0, 0) { }

    public static implicit operator PixelPoint(DlaPixel pixel)
    {
        return new(pixel.X, pixel.Y);
    }
}
