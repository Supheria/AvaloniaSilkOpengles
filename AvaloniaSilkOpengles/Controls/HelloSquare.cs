using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using AvaloniaSilkOpengles.Assets.Shaders;
using AvaloniaSilkOpengles.Assets.Textures;
using Silk.NET.OpenGLES;
using SkiaSharp;
using StbImageSharp;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Controls;

public unsafe class HelloSquare : OpenGlControlBase
{
    int VaoHandler { get; set; }
    int VboHandler { get; set; }
    int TexCoordVboHandler { get; set; }
    int EboHandler { get; set; }
    Shader? Shader { get; set; }
    Camera2D? Camera { get; set; }
    PixelSize ViewPortSize { get; set; }
    Texture? HappyTexture { get; set; }
    float RotaionY { get; set; }

    // csharpier-ignore
    float[] Vertices { get; } =
    [
        -0.5f, 0.5f, 0f,    1f, 0f, 0f, 1f,     0f, 1f, // top left
        0.5f, 0.5f, 0f,     0f, 1f, 0f, 1f,     1f, 1f, // top right
        0.5f, -0.5f, 0f,    0f, 1f, 1f, 1f,     1f, 0f, // bottom right
        -0.5f, -0.5f, 0f,   0f, 0f, 1f, 1f,     0f, 0f, // bottom left
    ];
    
    // List<Vector3> Vertices

    // // a way no need to flip picture vertically
    // // csharpier-ignore
    // float[] TexCoords { get; } =
    // [
    //     0f, 0f, // top left
    //     1f, 0f, // top right
    //     1f, 1f, // bottom right
    //     0f, 1f, // bottom left
    // ];

    // csharpier-ignore
    float[] TexCoords { get; } =
    [
        0f, 1f, // top left
        1f, 1f, // top right
        1f, 0f, // bottom right
        0f, 0f // bottom left
    ];

    // csharpier-ignore
    byte[] Indices { get; } =
    [
        0, 1, 2, // top right part
        2, 3, 0, // bottom left part
    ];

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        Camera = new(Bounds.Center, 2.5f);
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var size = e.NewSize * scaling;
        ViewPortSize = new((int)size.Width, (int)size.Height);
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);

        using var Gl = GL.GetApi(gl.GetProcAddress);

        Shader = new(ShaderRead.Read("simple.vert"), ShaderRead.Read("simple.frag"));
        Shader.Load(gl);

        VaoHandler = gl.GenVertexArray();
        VboHandler = gl.GenBuffer();
        EboHandler = gl.GenBuffer();

        gl.BindVertexArray(VaoHandler);
        gl.BindBuffer(GL_ARRAY_BUFFER, VboHandler);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, EboHandler);

        fixed (float* v = &Vertices[0])
        {
            gl.BufferData(
                GL_ARRAY_BUFFER,
                sizeof(float) * Vertices.Length,
                new IntPtr(v),
                GL_STATIC_DRAW
            );
        }
        fixed (byte* v = &Indices[0])
        {
            gl.BufferData(
                GL_ELEMENT_ARRAY_BUFFER,
                sizeof(byte) * Indices.Length,
                new IntPtr(v),
                GL_STATIC_DRAW
            );
        }

        gl.VertexAttribPointer(0, 3, GL_FLOAT, 0, sizeof(float) * 9, IntPtr.Zero);
        gl.EnableVertexAttribArray(0);

        gl.VertexAttribPointer(1, 4, GL_FLOAT, 0, sizeof(float) * 9, sizeof(float) * 3);
        gl.EnableVertexAttribArray(1);

        TexCoordVboHandler = gl.GenBuffer();
        gl.BindBuffer(GL_ARRAY_BUFFER, TexCoordVboHandler);
        fixed (float* v = &TexCoords[0])
        {
            gl.BufferData(
                GL_ARRAY_BUFFER,
                sizeof(float) * TexCoords.Length,
                new IntPtr(v),
                GL_STATIC_DRAW
            );
        }

        // gl.VertexAttribPointer(2, 2, GL_FLOAT, 0, sizeof(float) * 8, sizeof(float) * 6);
        gl.VertexAttribPointer(2, 2, GL_FLOAT, 0, sizeof(float) * 2, IntPtr.Zero);
        gl.EnableVertexAttribArray(2);

        gl.BindVertexArray(0);
        gl.BindBuffer(GL_ARRAY_BUFFER, 0);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

        HappyTexture = new(gl);
        using var stream = TextureRead.Read("happy.jpg");
        HappyTexture.Load(stream);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
        gl.DeleteVertexArray(VaoHandler);
        gl.DeleteBuffer(VboHandler);
        gl.DeleteBuffer(TexCoordVboHandler);
        gl.DeleteBuffer(EboHandler);
        Shader?.Delete(gl);
        HappyTexture?.Delete();
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (Shader is null || Camera is null)
            return;

        gl.Viewport(0, 0, ViewPortSize.Width, ViewPortSize.Height);

        gl.ClearColor(0, 0, 0, 1);
        gl.Clear(GL_COLOR_BUFFER_BIT);

        // var position = Bounds.Center;
        // var scale = new Vector2(150, 100);
        // var rotation = MathF.Sin(St.Elapsed.Milliseconds) * MathF.PI;
        // var trans = Matrix4x4.CreateTranslation((float)position.X, (float)position.Y, 0);
        // var sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
        // var rot = Matrix4x4.CreateRotationZ(rotation);
        // Shader.SetMatrix(gl, "model", sca * rot * trans);
        // Shader.SetMatrix(gl, "model", sca * trans);

        var model = Matrix4x4.Identity;
        var view = Matrix4x4.Identity;
        var projection = Camera.GetProjectionMatrix(Bounds);

        var rotation = Matrix4x4.CreateRotationY(RotaionY);
        var translation = Matrix4x4.CreateTranslation(0f, 0f, -3f);
        model = rotation * translation;
        RotaionY += 0.01f;

        Shader.SetMatrix(gl, "model", model);
        Shader.SetMatrix(gl, "view", view);
        Shader.SetMatrix(gl, "projection", projection);

        Shader.Use(gl);

        HappyTexture?.Bind();

        gl.BindVertexArray(VaoHandler);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, EboHandler);
        // gl.DrawArrays(GL_TRIANGLES, 0, 3);
        gl.DrawElements(GL_TRIANGLES, Indices.Length, GL_UNSIGNED_BYTE, IntPtr.Zero);

        gl.BindVertexArray(0);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        HappyTexture?.Unbind();

        // Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
    }
}
