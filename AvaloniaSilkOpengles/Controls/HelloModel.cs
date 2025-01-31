using AvaloniaSilkOpengles.Assets.Models;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class HelloModel : SilkNetOpenGlControl
{
    LightCube? LightCube { get; set; }
    Model? Model1 { get; set; }
    Model? Model2 { get; set; }
    Model? Crow { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? FrogShader { get; set; }
    ShaderHandler? OutlineShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        LightCube = new(gl);
        Model1 = new(gl, new ModelRead("trees"));
        Model2 = new(gl, new ModelRead("ground"));
        Crow = new(gl, new ModelRead("crow"));

        LightShader = ShaderHandler.Create(gl, "light", []);
        FrogShader = ShaderHandler.Create(gl, "frog", []);
        OutlineShader = ShaderHandler.Create(gl, "outline", []);

        gl.Enable(EnableCap.DepthTest);
        gl.DepthFunc(DepthFunction.Less);

        gl.FrontFace(FrontFaceDirection.Ccw);
        // gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
        
        Camera.SetPosition(new(0, 2, 0));
    }

    protected override void OnGlDeinit(GL gl)
    {
        Model1?.Delete(gl);
        Model2?.Delete(gl);
        Crow?.Delete(gl);

        LightCube?.Delete(gl);

        FrogShader?.Delete(gl);
        LightShader?.Delete(gl);
    }

    protected override void OnGlRender(GL gl)
    {
        if (LightCube is null || FrogShader is null || LightShader is null)
            return;
        var backGround = new Vector4(0.85f, 0.85f, 0.90f, 1.0f);
        gl.ClearColor(backGround.X, backGround.Y, backGround.Z, backGround.W);
        gl.Clear(
            ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit
                | ClearBufferMask.StencilBufferBit
        );
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        // var lightPos = new Vector3(3.0f, 4.0f, -3.0f);
        var lightPos = new Vector3(5.0f, 9.0f, 5.0f);
        LightCube.Position = lightPos;
        LightCube.Scale = new(0.05f, 0.05f, 0.05f);

        LightShader.Use(gl);
        LightShader.UniformVector4(gl, "lightColor", lightColor);

        LightCube?.Render(gl, LightShader, Camera);

        FrogShader.Use(gl);
        FrogShader.UniformVector4(gl, "lightColor", lightColor);
        FrogShader.UniformVector3(gl, "lightPos", lightPos);
        FrogShader.UniformVector4(gl, "backGround", backGround);
        Model1?.Render(gl, FrogShader, Camera);
        Model2?.Render(gl, FrogShader, Camera);
        
        gl.Disable(EnableCap.CullFace);
        Crow?.Render(gl, FrogShader, Camera);
        gl.Enable(EnableCap.CullFace);
    }
}