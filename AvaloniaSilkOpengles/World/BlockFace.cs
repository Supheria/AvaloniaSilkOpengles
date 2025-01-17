using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public struct BlockFace
{
    public Vector3[] Vertices { get; set; }
    public Vector2[] Uvs { get; set; }
    
    public BlockFace(Vector3[] vertices, Vector2[] uvs)
    {
        Vertices = vertices;
        Uvs = uvs;
    }
}
