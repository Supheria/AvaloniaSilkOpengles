using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.General;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal sealed class CornerBorderNode(VoronoiVertex point) : BorderNode
{
    public override Direction BorderLocation { get; } = point.DirectionOnBorder;

    public override VoronoiVertex Vertex { get; } = point;

    public override double Angle => throw new InvalidOperationException();

    public override int FallbackComparisonIndex => throw new InvalidOperationException();
}
