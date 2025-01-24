using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using LocalUtilities.General;

namespace AvaloniaSilkOpengles.World2;

public sealed class TerrainFace : Face
{
    public TerrainFace(IList<Position> positions, RenderColor color)
    {
        var normal = GetNormal(positions);
        foreach (var coord in positions)
            AddVertex(coord, normal, color, TexUv.Zero);
    }

    private Normal GetNormal(IList<Position> positions)
    {
        var u = positions[2].Value - positions[1].Value;
        var v = positions[0].Value - positions[1].Value;
        var uxvX = u.Y * v.Z - u.Z * v.Y;
        var uxvY = u.Z * v.X - u.X * v.Z;
        var uxvZ = u.X * v.Y - u.Y * v.X;
        return new(uxvX, uxvY, uxvZ);
    }
}
