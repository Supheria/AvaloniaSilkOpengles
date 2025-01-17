using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class Block
{
    public Vector3 Position { get; }
    public BlockType Type { get; }
    Dictionary<Face, TexCoord> TexCoords { get; } = [];
    Dictionary<Face, BlockFace> Faces { get; } = [];

    public BlockFace this[Face face] => Faces[face];

    public Block(Vector3 position, BlockType type)
    {
        Position = position;
        Type = type;
        if (type is not BlockType.Empty)
        {
            TexCoords = TextureData.BlockTexCoordData[type];
            Faces = new()
            {
                [Face.Front] = GetFace(Face.Front, position, TexCoords[Face.Front]),
                [Face.Back] = GetFace(Face.Back, position, TexCoords[Face.Back]),
                [Face.Left] = GetFace(Face.Left, position, TexCoords[Face.Left]),
                [Face.Right] = GetFace(Face.Right, position, TexCoords[Face.Right]),
                [Face.Top] = GetFace(Face.Top, position, TexCoords[Face.Top]),
                [Face.Bottom] = GetFace(Face.Bottom, position, TexCoords[Face.Bottom]),
            };
        }
    }

    private static BlockFace GetFace(Face face, Vector3 position, TexCoord texCoord)
    {
        var vertices = FaceData.RawVertexData[face].Select(v => v + position).ToArray();
        return new(vertices, texCoord.Uvs);
    }
}
