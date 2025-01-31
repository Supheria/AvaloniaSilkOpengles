using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Assets;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;
using StbImageSharp;

namespace AvaloniaSilkOpengles.World;

public sealed class Chunk : GameObject
{
    public Chunk(GL gl)
    {
        SetCurrentModel(gl, new ChunkModel(gl));
        AddTextures(gl);
    }

    private void AddTextures(GL gl)
    {
        // using var source1 = AssetsRead.ReadTexture("planks");
        // using var source2 = AssetsRead.ReadTexture("planksSpec");
        // var texture = ImageResult.FromStream(source1, ColorComponents.RedGreenBlueAlpha);
        var diffuse = Texture2D.Create(gl, "planks", 0, TextureType.Diffuse);
        // texture = ImageResult.FromStream(source2, ColorComponents.RedGreenBlueAlpha);
        var specular = Texture2D.Create(gl, "planksSpec", 1, TextureType.Specular);
        SetTextures(diffuse, specular);
    }
}
