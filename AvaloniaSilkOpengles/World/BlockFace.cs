using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;

namespace AvaloniaSilkOpengles.World;

public struct BlockFace
{
    public List<VertexTexture> Vertices { get; } = [];

    public BlockFace(IList<Vector3> coords, IList<Vector3> normals, IList<Vector2> uvs)
    {
        for (var i = 0; i < coords.Count; i++)
        {
            var vertex = new VertexTexture(coords[i], normals[i], uvs[i]);
            Vertices.Add(vertex);
        }
    }
}
