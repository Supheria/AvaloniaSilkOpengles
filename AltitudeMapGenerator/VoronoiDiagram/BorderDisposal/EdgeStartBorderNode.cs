using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.General;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal sealed class EdgeStartBorderNode(VoronoiEdge edge, int fallbackComparisonIndex)
    : EdgeBorderNode(edge, fallbackComparisonIndex)
{
    public override Direction BorderLocation => Edge.Starter.DirectionOnBorder;

    public override VoronoiVertex Vertex => Edge.Starter;

    public override double Angle => Vertex.AngleTo(Edge.Ender); // away from border
}
