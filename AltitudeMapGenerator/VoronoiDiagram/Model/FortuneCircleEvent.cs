using AltitudeMapGenerator.VoronoiDiagram.Data;

namespace AltitudeMapGenerator.VoronoiDiagram.Model;

internal sealed class FortuneCircleEvent : IFortuneEvent
{
    public VoronoiVertex Lowest { get; }
    public double YCenter { get; }
    public RBTreeNode<BeachSection> ToDelete { get; }

    public FortuneCircleEvent(
        VoronoiVertex lowest,
        double yCenter,
        RBTreeNode<BeachSection> toDelete
    )
    {
        Lowest = lowest;
        YCenter = yCenter;
        ToDelete = toDelete;
    }

    public int CompareTo(IFortuneEvent? other)
    {
        if (other is null)
        {
            if (this is null)
                return 0;
            else
                throw VoronoiException.NullFortuneCircleEvent(nameof(other));
        }
        int c = Y.CompareTo(other.Y);
        return c == 0 ? X.CompareTo(other.X) : c;
    }

    public double X => Lowest.X;
    public double Y => Lowest.Y;
}
