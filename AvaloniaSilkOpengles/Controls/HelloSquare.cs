using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using AvaloniaSilkOpengles.Assets.Models;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;
using Vector = Avalonia.Vector;

namespace AvaloniaSilkOpengles.Controls;

public class HelloSquare : SilkNetOpenGlControl
{
    LightCube? LightCube { get; set; }
    Chunk? Chunk { get; set; }
    Model? Model1 { get; set; }
    Model? Model2 { get; set; }
    Model? Crow { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? SimpleShader { get; set; }
    ShaderHandler? OutlineShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        LightCube = new(gl);
        Chunk = new(gl);
        Model1 = new(gl, new ModelRead("trees"));
        Model2 = new(gl, new ModelRead("ground"));
        Crow = new(gl, new ModelRead("crow"));

        LightShader = new(gl, "light");
        SimpleShader = new(gl, "simple");
        OutlineShader = new(gl, "outline");

        gl.Enable(EnableCap.DepthTest);
        gl.DepthFunc(DepthFunction.Less);

        gl.FrontFace(FrontFaceDirection.Ccw);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
    }

    protected override void OnGlDeinit()
    {
        Model1?.Delete();
        Model2?.Delete();
        Crow?.Delete();

        LightCube?.Delete();
        Chunk?.Delete();

        SimpleShader?.Delete();
        LightShader?.Delete();
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
        // var lightPos = new Vector3(3.0f, 4.0f, -3.0f);
        var lightPos = new Vector3(5.0f, 9.0f, 5.0f);
        LightCube.Translation = lightPos;
        LightCube.Scale = new(0.05f, 0.05f, 0.05f);

        LightShader.Use();
        LightShader.SetVector4("lightColor", lightColor);

        LightCube?.Render(LightShader, Camera);

        SimpleShader.Use();
        SimpleShader.SetVector4("lightColor", lightColor);
        SimpleShader.SetVector3("lightPos", lightPos);
        SimpleShader.SetVector4("backGround", backGround);
        // Model1?.Render(SimpleShader, Camera);
        // Model2?.Render(SimpleShader, Camera);
        // Crow?.Render(SimpleShader, Camera);
        Chunk?.Render(SimpleShader, Camera);
    }
}
