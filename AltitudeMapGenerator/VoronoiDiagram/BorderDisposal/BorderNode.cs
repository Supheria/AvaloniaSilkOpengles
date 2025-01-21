using AltitudeMapGenerator.Extensions;
using AltitudeMapGenerator.VoronoiDiagram.Data;
using LocalUtilities.General;

namespace AltitudeMapGenerator.VoronoiDiagram.BorderDisposal;

internal abstract class BorderNode
{
    public abstract Direction BorderLocation { get; }

    public abstract VoronoiVertex Vertex { get; }

    public abstract double Angle { get; }

    public abstract int FallbackComparisonIndex { get; }

    public int CompareAngleTo(BorderNode node2, Direction pointBorderLocation)
    {
        // "Normal" Atan2 returns an angle between -π ≤ θ ≤ π as "seen" on the Cartesian plane,
        // that is, starting at the "right" of x axis and increasing counter-clockwise.
        // But we want the angle sortable (counter-)clockwise along each side.
        // So we cannot have the origin be "crossable" by the angle.

        //             0..-π or π
        //             ↓←←←←←←←←
        //             ↓       ↑  π/2..π
        //  -π/2..π/2  X       O  -π/2..-π
        //             ↑       ↓
        //             ↑←←←←←←←←
        //             0..π or -π

        // Now we need to decide how to compare them based on the side

        double angle1 = Angle;
        double angle2 = node2.Angle;

        switch (pointBorderLocation)
        {
            case Direction.Left:
                // Angles are -π/2..π/2
                // We don't need to adjust to have it in the same directly-comparable range
                // Smaller angle comes first
                break;
            case Direction.Bottom:
                // Angles are 0..-π or π
                // We can swap π to -π
                // Smaller angle comes first
                if (angle1.ApproxGreaterThan(0))
                    angle1 -= 2 * Math.PI;
                if (angle2.ApproxGreaterThan(0))
                    angle2 -= 2 * Math.PI;
                break;
            case Direction.Right:
                // Angles are π/2..π or -π/2..-π
                // We can swap <0 to >0
                // Angles are now π/2..π or 3/2π..π, i.e. π/2..3/2π
                if (angle1.ApproxLessThan(0))
                    angle1 += 2 * Math.PI;
                if (angle2.ApproxLessThan(0))
                    angle2 += 2 * Math.PI;
                break;
            case Direction.Top:
                // Angles are 0..π or -π
                // We can swap -π to π
                // Smaller angle comes first
                if (angle1.ApproxLessThan(0))
                    angle1 += 2 * Math.PI;
                if (angle2.ApproxLessThan(0))
                    angle2 += 2 * Math.PI;
                break;
            case Direction.BottomRight:
            case Direction.TopRight:
            case Direction.LeftBottom:
            case Direction.LeftTop:
            case Direction.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(pointBorderLocation),
                    pointBorderLocation,
                    null
                );
        }

        // Smaller angle comes first
        return angle1.ApproxCompareTo(angle2);
    }
}
