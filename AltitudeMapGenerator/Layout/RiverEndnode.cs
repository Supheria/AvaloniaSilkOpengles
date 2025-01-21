using AltitudeMapGenerator.Extensions;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using Avalonia;
using LocalUtilities.General;

namespace AltitudeMapGenerator.Layout;

internal sealed class RiverEndnode(
    Direction direction,
    OperatorType operatorTypeType,
    PixelSize size
)
{
    public Direction Direction { get; } = direction;

    public OperatorType OperatorTypeType { get; } = operatorTypeType;

    public double CompareValue { get; } =
        direction switch
        {
            Direction.Left or Direction.Right => size.Height / 2d,
            Direction.Top or Direction.Bottom => size.Width / 2d,
            _ => throw AltitudeMapGeneratorException.NotProperRiverEndnodeDirection(direction),
        };

    public bool VoronoiVertexFilter(VoronoiVertex vertex)
    {
        if (vertex.DirectionOnBorder != Direction)
            return false;
        var value = Direction switch
        {
            Direction.Left or Direction.Right => vertex.Y,
            Direction.Top or Direction.Bottom => vertex.X,
            _ => throw AltitudeMapGeneratorException.NotProperRiverEndnodeDirection(Direction),
        };
        return value.ApproxOperatorTo(OperatorTypeType, CompareValue);
    }
}
