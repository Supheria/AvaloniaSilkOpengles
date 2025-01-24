using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public sealed class TextureData
{
    public static Dictionary<BlockType, Dictionary<FaceType, TexCoord>> BlockTexCoordData { get; } =
        new()
        {
            [BlockType.Dirt] = new()
            {
                [FaceType.Front] = TexCoord.CreateFromAtlas(2, 0),
                [FaceType.Right] = TexCoord.CreateFromAtlas(2, 0),
                [FaceType.Back] = TexCoord.CreateFromAtlas(2, 0),
                [FaceType.Left] = TexCoord.CreateFromAtlas(2, 0),
                [FaceType.Top] = TexCoord.CreateFromAtlas(2, 0),
                [FaceType.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
            [BlockType.Grass] = new()
            {
                [FaceType.Front] = TexCoord.CreateFromAtlas(3, 0),
                [FaceType.Right] = TexCoord.CreateFromAtlas(3, 0),
                [FaceType.Back] = TexCoord.CreateFromAtlas(3, 0),
                [FaceType.Left] = TexCoord.CreateFromAtlas(3, 0),
                [FaceType.Top] = TexCoord.CreateFromAtlas(7, 2),
                [FaceType.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
            [BlockType.CuttenPumpkin] = new()
            {
                [FaceType.Front] = TexCoord.CreateFromAtlas(7, 7),
                [FaceType.Right] = TexCoord.CreateFromAtlas(6, 7),
                [FaceType.Back] = TexCoord.CreateFromAtlas(6, 7),
                [FaceType.Left] = TexCoord.CreateFromAtlas(6, 7),
                [FaceType.Top] = TexCoord.CreateFromAtlas(6, 6),
                [FaceType.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
            [BlockType.Glass] = new()
            {
                [FaceType.Front] = TexCoord.CreateFromSingle(),
                [FaceType.Right] = TexCoord.CreateFromSingle(),
                [FaceType.Back] = TexCoord.CreateFromSingle(),
                [FaceType.Left] = TexCoord.CreateFromSingle(),
                [FaceType.Top] = TexCoord.CreateFromSingle(),
                [FaceType.Bottom] = TexCoord.CreateFromSingle(),
            },
            [BlockType.TestBlock] = new()
            {
                [FaceType.Front] = TexCoord.CreateFromSingle(),
                [FaceType.Right] = TexCoord.CreateFromSingle(),
                [FaceType.Back] = TexCoord.CreateFromSingle(),
                [FaceType.Left] = TexCoord.CreateFromSingle(),
                [FaceType.Top] = TexCoord.CreateFromSingle(),
                [FaceType.Bottom] = TexCoord.CreateFromSingle(),
            }
        };
}
