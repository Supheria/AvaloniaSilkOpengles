using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class ChunkLoader : SilkNetOpenGlControl
{
    LightCube? LightCube { get; set; }
    Chunk? Chunk { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? SimpleShader { get; set; }
    ShaderHandler? OutlineShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        LightCube = new(gl);
        Chunk = new(gl);

        LightShader = ShaderHandler.Create(gl, "light", []);
        SimpleShader = ShaderHandler.Create(gl, "simple", []);
        OutlineShader = ShaderHandler.Create(gl, "outline", []);

        gl.Enable(EnableCap.DepthTest);
        gl.DepthFunc(DepthFunction.Less);

        gl.FrontFace(FrontFaceDirection.Ccw);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        
        Camera.SetPosition(new(5.0f, 9.0f, 7.0f));
    }

    protected override void OnGlDeinit(GL gl)
    {
        LightCube?.Delete(gl);
        Chunk?.Delete(gl);

        SimpleShader?.Delete(gl);
        LightShader?.Delete(gl);
    }

    protected override void OnGlRender(GL gl)
    {
        if (LightCube is null || SimpleShader is null || LightShader is null)
            return;
        var backGround = new Vector4(0.85f, 0.85f, 0.90f, 1.0f);
        gl.ClearColor(backGround.X, backGround.Y, backGround.Z, backGround.W);
        gl.Clear(
            ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit
                | ClearBufferMask.StencilBufferBit
        );
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        var lightPos = new Vector3(5.0f, 9.0f, 5.0f);
        LightCube.Position = lightPos;
        LightCube.Scale = new(0.05f, 0.05f, 0.05f);

        LightShader.Use(gl);
        LightShader.UniformVector4(gl, "lightColor", lightColor);
        LightCube?.Render(gl, LightShader, Camera);

        SimpleShader.Use(gl);
        SimpleShader.UniformVector4(gl, "lightColor", lightColor);
        SimpleShader.UniformVector3(gl, "lightPos", lightPos);
        SimpleShader.UniformVector4(gl, "backGround", backGround);
        Chunk?.Render(gl, SimpleShader, Camera);
    }
}
