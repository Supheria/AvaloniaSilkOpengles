using System.Numerics;
using LocalUtilities.General;

namespace AvaloniaSilkOpengles.World2;

public readonly record struct TerrainCoord(VertexIndex Index, Vector3 Coord) : IRosterItem<VertexIndex>
{
    public VertexIndex Signature => Index;
}