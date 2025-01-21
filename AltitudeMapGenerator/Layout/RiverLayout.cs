namespace AltitudeMapGenerator.Layout;

internal sealed class RiverLayout(params (RiverEndnode Start, RiverEndnode Finish)[] endnodePairs)
{
    public (RiverEndnode Start, RiverEndnode Finish) this[int index] => Layout[index];

    public List<(RiverEndnode Start, RiverEndnode Finish)> Layout { get; } = endnodePairs.ToList();
}
