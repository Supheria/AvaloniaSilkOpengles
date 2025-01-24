using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AvaloniaSilkOpengles.Assets.Textures;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Sphere;

public class SelectableSphere : GameObject
{
    Texture2DHandler Texture1 { get; set; }
    Texture2DHandler Texture2 { get; set; }

    public SelectableSphere(GL gl, int recursionLevel)
    {
        SetModelRecursionLevel(gl, recursionLevel);
        using var stream1 = TextureRead.Read("wood");
        Texture1 = new(gl, stream1, TextureType.Diffuse, 0);
        using var stream2 = TextureRead.Read("moon");
        Texture2 = new(gl, stream2, TextureType.Diffuse, 0);
        Textures.Add(Texture1);
    }

    public void SetModelRecursionLevel(GL gl, int recursionLevel)
    {
        SetCurrentModel(gl, new IcoSphereModel(gl, recursionLevel));
    }
}
