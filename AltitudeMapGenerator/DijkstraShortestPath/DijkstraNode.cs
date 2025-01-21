using Avalonia;

namespace AltitudeMapGenerator.DijkstraShortestPath;

internal sealed class DijkstraNode(PixelPoint node, int index)
{
    internal bool Used { get; set; }
    internal List<PixelPoint> Nodes { get; } = [];

    internal PixelPoint PixelPoint { get; set; } = node;

    internal int Index { get; set; } = index;

    internal double Weight { get; set; } = double.MaxValue;
}
