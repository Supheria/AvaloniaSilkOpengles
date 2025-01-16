using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public sealed class Block
{
    public Vector3 Position { get; }
    public BlockType Type { get; }
    Dictionary<Face, BlockFace> Faces { get; }

    public BlockFace this[Face face] => Faces[face];

    public Block(Vector3 position, BlockType type)
    {
        Position = position;
        Type = type;
        Faces = new()
        {
            [Face.Front] = GetFace(Face.Front, position),
            [Face.Back] = GetFace(Face.Back, position),
            [Face.Left] = GetFace(Face.Left, position),
            [Face.Right] = GetFace(Face.Right, position),
            [Face.Top] = GetFace(Face.Top, position),
            [Face.Bottom] = GetFace(Face.Bottom, position),
        };
    }

    private static BlockFace GetFace(Face face, Vector3 position)
    {
        var vertices = FaceData.RawVertexData[face].Select(v => v + position).ToList();
        var uv = new List<Vector2> { new(0f, 1f), new(1f, 1f), new(1f, 0f), new(0f, 0f) };
        return new(vertices, uv);
    }
}
