using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Assets.Textures;
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
        using var source1 = TextureRead.Read("planks");
        using var source2 = TextureRead.Read("planksSpec");
        // var texture = ImageResult.FromStream(source1, ColorComponents.RedGreenBlueAlpha);
        var diffuse = new Texture2DHandler(gl, source1, TextureType.Diffuse, 0);
        // texture = ImageResult.FromStream(source2, ColorComponents.RedGreenBlueAlpha);
        var specular = new Texture2DHandler(gl, source2, TextureType.Specular, 1);
        SetTextures(diffuse, specular);
    }
}
