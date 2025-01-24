using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.World;

public class LightCubeFace : Face
{
    public LightCubeFace(IList<Position> positions, RenderColor color)
    {
        foreach (var position in positions)
            AddVertex(position, Normal.Zero, color, TexUv.Zero);
    }
}
