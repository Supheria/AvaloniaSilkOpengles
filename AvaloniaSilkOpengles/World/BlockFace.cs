using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public struct BlockFace
{
    public List<Vector3> Vertices { get; set; }
    public List<Vector2> Uvs { get; set; }
    
    public BlockFace(List<Vector3> vertices, List<Vector2> uvs)
    {
        Vertices = vertices;
        Uvs = uvs;
    }
}
