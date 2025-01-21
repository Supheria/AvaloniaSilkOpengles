using AltitudeMapGenerator.VoronoiDiagram.Data;
using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator.DLA;

internal sealed class DlaMap(VoronoiCell cell)
{
    Dictionary<PixelPoint, DlaPixel> PixelMap { get; } = [];

    VoronoiCell Cell { get; set; } = cell;

    PixelRect Bounds { get; set; } = cell.GetBounds();

    internal double AltitudeMax { get; private set; } = 0;

    public static IProgressor? Progressor { get; set; }

    //#endif
    /// <summary>
    ///
    /// </summary>
    /// <param name="pixelCount"></param>
    /// <param name="density">[0,1], bigger means that grid-shape is closer to voronoi-cells' shape</param>
    /// <returns></returns>
    internal List<DlaPixel> Generate(int pixelCount, double density)
    {
        PixelMap.Clear();
        AltitudeMax = 0;
        var root = new PixelPoint(Cell.Site.X, Cell.Site.Y);
        PixelMap[root] = new(root.X, root.Y);
        bool innerFilter(int x, int y) => Cell.ContainPoint(x, y);
        var count = (int)(pixelCount * density);
        for (int i = 0; PixelMap.Count < count; i++)
        {
            var pixel = AddWalker(innerFilter);
            PixelMap[pixel] = pixel;
            Progressor?.Progress();
        }
        bool outerFilter(int x, int y) => Bounds.Contains(new PixelPoint(x, y));
        for (int i = 0; PixelMap.Count < pixelCount; i++)
        {
            var pixel = AddWalker(outerFilter);
            PixelMap[pixel] = pixel;
            Progressor?.Progress();
        }
        ComputeHeight();
        return PixelMap.Values.ToList();
    }

    private DlaPixel AddWalker(Func<int, int, bool> pixelFilter)
    {
        var pixel = new DlaPixel(
            new Random().Next(Bounds.X, Bounds.Right + 1),
            new Random().Next(Bounds.Y, Bounds.Bottom + 1)
        );
        while (!CheckStuck(pixel))
        {
            int x = pixel.X,
                y = pixel.Y;
            switch (new Random().Next(0, 8))
            {
                case 0: // left
                    x--;
                    break;
                case 1: // right
                    x++;
                    break;
                case 2: // up
                    y--;
                    break;
                case 3: // down
                    y++;
                    break;
                case 4: // left up
                    x--;
                    y--;
                    break;
                case 5: // up right
                    x++;
                    y--;
                    break;
                case 6: // bottom right
                    x++;
                    y++;
                    break;
                case 7: // left bottom
                    x--;
                    y++;
                    break;
            }
            if (pixelFilter(x, y))
                pixel = new(x, y);
            else
                pixel = new(
                    new Random().Next(Bounds.X, Bounds.Right + 1),
                    new Random().Next(Bounds.Y, Bounds.Bottom + 1)
                );
        }
        return pixel;
    }

    private bool CheckStuck(DlaPixel pixel)
    {
        var X = pixel.X;
        var Y = pixel.Y;
        var left = X - 1; //Math.Max(x - 1, Bounds.Left);
        var top = Y - 1; //Math.Max(y - 1, Bounds.Top);
        var right = X + 1; //Math.Min(x + 1, Bounds.Right);
        var bottom = Y + 1; //Math.Min(y + 1, Bounds.Bottom);
        bool isStucked = false;
        if (PixelMap.ContainsKey(new(X, Y)))
            return false;
        if (PixelMap.TryGetValue(new(left, Y), out var stucked))
        {
            pixel.Neighbor[Direction.Left] = new(left, Y);
            stucked.Neighbor[Direction.Right] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, Y), out stucked))
        {
            pixel.Neighbor[Direction.Right] = new(right, Y);
            stucked.Neighbor[Direction.Left] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(X, top), out stucked))
        {
            pixel.Neighbor[Direction.Top] = new(X, top);
            stucked.Neighbor[Direction.Bottom] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(X, bottom), out stucked))
        {
            pixel.Neighbor[Direction.Bottom] = new(X, bottom);
            stucked.Neighbor[Direction.Top] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(left, top), out stucked))
        {
            pixel.Neighbor[Direction.LeftTop] = new(left, top);
            stucked.Neighbor[Direction.BottomRight] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(left, bottom), out stucked))
        {
            pixel.Neighbor[Direction.LeftBottom] = new(left, bottom);
            stucked.Neighbor[Direction.TopRight] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, top), out stucked))
        {
            pixel.Neighbor[Direction.TopRight] = new(right, top);
            stucked.Neighbor[Direction.LeftBottom] = new(X, Y);
            isStucked = true;
        }
        if (PixelMap.TryGetValue(new(right, bottom), out stucked))
        {
            pixel.Neighbor[Direction.BottomRight] = new(right, bottom);
            stucked.Neighbor[Direction.LeftTop] = new(X, Y);
            isStucked = true;
        }
        return isStucked;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="pixelMap"></param>
    /// <returns>the max of heights</returns>
    private void ComputeHeight()
    {
        foreach (var pair in PixelMap)
        {
            var pixel = pair.Value;
            CheckDirection(Direction.Left, pixel);
            CheckDirection(Direction.Top, pixel);
            CheckDirection(Direction.Right, pixel);
            CheckDirection(Direction.Bottom, pixel);
            CheckDirection(Direction.LeftTop, pixel);
            CheckDirection(Direction.TopRight, pixel);
            CheckDirection(Direction.LeftBottom, pixel);
            CheckDirection(Direction.BottomRight, pixel);
            AltitudeMax = Math.Max(AltitudeMax, pixel.Altitude);
        }
    }

    private int CheckDirection(Direction direction, DlaPixel walker)
    {
        if (!walker.ConnetNumber.ContainsKey(direction))
        {
            if (walker.Neighbor.TryGetValue(direction, out var neighbor))
                walker.ConnetNumber[direction] = CheckDirection(direction, PixelMap[neighbor]) + 1;
            else
                walker.ConnetNumber[direction] = 0;
        }
        return walker.ConnetNumber[direction];
    }
}
