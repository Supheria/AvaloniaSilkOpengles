using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World2;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class TerrainLoader : SilkNetOpenGlControl
{
    Terrain? TestTerrain { get; set; }
    ShaderHandler? TerrainShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        TestTerrain = new(gl);
        TerrainShader = new(gl, "terrain");
        
        gl.Enable(EnableCap.DepthTest);
        
        gl.FrontFace(FrontFaceDirection.Ccw);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
    }

    protected override void OnGlDeinit()
    {
        TestTerrain?.Delete();
        TerrainShader?.Delete();
    }

    protected override void OnGlRender(GL gl)
    {
        if (TestTerrain is null || TerrainShader is null)
            return;

        gl.ClearColor(0.85f, 0.85f, 0.90f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        TerrainShader.Use();
        TestTerrain.Render(
            TerrainShader,
            Camera,
            new(0.1f, 0.1f, 0.1f),
            Quaternion.Identity,
            Vector3.Zero,
            Matrix4x4.Identity
        );
    }
}
