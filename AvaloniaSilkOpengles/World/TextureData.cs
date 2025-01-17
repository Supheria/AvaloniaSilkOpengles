using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public sealed class TextureData
{
    public static Dictionary<BlockType, Dictionary<Face, TexCoord>> BlockTexCoordData { get; } =
        new()
        {
            [BlockType.Dirt] = new()
            {
                [Face.Front] = TexCoord.CreateFromAtlas(2, 0),
                [Face.Back] = TexCoord.CreateFromAtlas(2, 0),
                [Face.Left] = TexCoord.CreateFromAtlas(2, 0),
                [Face.Right] = TexCoord.CreateFromAtlas(2, 0),
                [Face.Top] = TexCoord.CreateFromAtlas(2, 0),
                [Face.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
            [BlockType.Grass] = new()
            {
                [Face.Front] = TexCoord.CreateFromAtlas(3, 0),
                [Face.Back] = TexCoord.CreateFromAtlas(3, 0),
                [Face.Left] = TexCoord.CreateFromAtlas(3, 0),
                [Face.Right] = TexCoord.CreateFromAtlas(3, 0),
                [Face.Top] = TexCoord.CreateFromAtlas(7, 2),
                [Face.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
            [BlockType.CuttenPumpkin] = new()
            {
                [Face.Front] = TexCoord.CreateFromAtlas(7, 7),
                [Face.Back] = TexCoord.CreateFromAtlas(6, 7),
                [Face.Left] = TexCoord.CreateFromAtlas(6, 7),
                [Face.Right] = TexCoord.CreateFromAtlas(6, 7),
                [Face.Top] = TexCoord.CreateFromAtlas(6, 6),
                [Face.Bottom] = TexCoord.CreateFromAtlas(2, 0),
            },
        };
}
