using System;
using Avalonia.Input;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World2;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class TerrainLoader : SilkNetOpenGlControl
{
    Terrain? TestTerrain { get; set; }
    ShaderHandler? TerrainShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        TestTerrain = new(gl);
        TerrainShader = ShaderHandler.Create(gl, "terrain", []);

        gl.Enable(EnableCap.DepthTest);

        gl.FrontFace(FrontFaceDirection.Ccw);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);

        Camera.SetPosition(new(0, 1, 0));
        Camera.SetRotation(0f, 45.0f);
    }

    protected override void OnGlDeinit(GL gl)
    {
        TestTerrain?.Delete(gl);
        TerrainShader?.Delete(gl);
    }

    protected override void OnGlRender(GL gl)
    {
        if (TestTerrain is null || TerrainShader is null)
            return;

        gl.ClearColor(0.85f, 0.85f, 0.90f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var scale = new Vector3(0.1f, 0.1f, 0.1f);
        TestTerrain.Scale = scale;

        TerrainShader.Use(gl);
        TestTerrain.Render(gl, TerrainShader, Camera);
    }
}
