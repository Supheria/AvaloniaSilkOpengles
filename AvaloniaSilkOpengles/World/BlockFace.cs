using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;

namespace AvaloniaSilkOpengles.World;

public class BlockFace : Face
{
    public BlockFace(IList<Position> positions, IList<Normal> normals, IList<TexUv> uvs)
    {
        for (var i = 0; i < positions.Count; i++)
            AddVertex(positions[i], normals[i], RenderColor.Zero, uvs[i]);
    }
}
