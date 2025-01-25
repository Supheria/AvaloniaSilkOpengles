using System.Collections.Generic;
using System.Linq;
using AvaloniaSilkOpengles.Graphics;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class Block
{
    public BlockType Type { get; }
    Dictionary<FaceType, TexCoord> TexCoords { get; } = [];
    Dictionary<FaceType, BlockFace> Faces { get; } = [];

    public BlockFace this[FaceType faceType] => Faces[faceType];

    public Block(Vector3 offset, BlockType type)
    {
        Type = type;
        if (type is not BlockType.Empty)
        {
            TexCoords = TextureData.BlockTexCoordData[type];
            Faces = new()
            {
                [FaceType.Front] = GetFace(FaceType.Front, offset, TexCoords[FaceType.Front]),
                [FaceType.Back] = GetFace(FaceType.Back, offset, TexCoords[FaceType.Back]),
                [FaceType.Left] = GetFace(FaceType.Left, offset, TexCoords[FaceType.Left]),
                [FaceType.Right] = GetFace(FaceType.Right, offset, TexCoords[FaceType.Right]),
                [FaceType.Top] = GetFace(FaceType.Top, offset, TexCoords[FaceType.Top]),
                [FaceType.Bottom] = GetFace(FaceType.Bottom, offset, TexCoords[FaceType.Bottom]),
            };
        }
    }

    private static BlockFace GetFace(FaceType faceType, Vector3 offset, TexCoord texCoord)
    {
        var positions = VertexData.Positions[faceType].Select(p => new Position(p.Value + offset)).ToArray();
        var normals = VertexData.Normals[faceType];
        return new(positions, normals, texCoord.Uvs);
    }
}
