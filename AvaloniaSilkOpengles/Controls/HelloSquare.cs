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
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Controls;

public unsafe class HelloSquare : OpenGlControlBase, ICustomHitTest
{
    GL? Gl {get;set;}
    VaoHandler? Vao { get; set; }
    VboHandler<Vector3>? PositionVbo { get; set; }
    VboHandler<Vector4>? ColorVbo { get; set; }
    VboHandler<Vector2>? TexCoordVbo { get; set; }
    EboHandler? Ebo { get; set; }
    ShaderHandler? Shader { get; set; }
    Camera3D? Camera { get; set; }
    Texture2DHandler? HappyTexture { get; set; }
    PixelSize ViewPortSize { get; set; }
    float RotationX { get; set; }
    float RotationY { get; set; }
    KeyEventArgs? KeyState { get; set; }
    Vector2 PointerPostionDiff {get;set;}
    Point LastPointerPostion { get; set; }

    List<Vertex> Vertices {get;} =
    [
        // front face
        new(new(-0.5f, 0.5f, 0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(0.5f, 0.5f, 0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(0.5f, -0.5f, 0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(-0.5f, -0.5f, 0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
        // right face
        new(new(0.5f, 0.5f, 0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(0.5f, 0.5f, -0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(0.5f, -0.5f, -0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(0.5f, -0.5f, 0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
        // back face
        new(new(0.5f, 0.5f, -0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(-0.5f, 0.5f, -0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(-0.5f, -0.5f, -0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(0.5f, -0.5f, -0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
        // left face
        new(new(-0.5f, 0.5f, -0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(-0.5f, 0.5f, 0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(-0.5f, -0.5f, 0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(-0.5f, -0.5f, -0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
        // top face
        new(new(-0.5f, 0.5f, -0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(0.5f, 0.5f, -0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(0.5f, 0.5f, 0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(-0.5f, 0.5f, 0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
        // bottom face
        new(new(-0.5f, -0.5f, 0.5f), new(1f, 0f, 0f, 1f), new(0f, 1f)),
        new(new(0.5f, -0.5f, 0.5f), new(0f, 1f, 0f, 1f), new(1f, 1f)),
        new(new(0.5f, -0.5f, -0.5f), new(0f, 1f, 1f, 1f), new(1f, 0f)),
        new(new(-0.5f, -0.5f, -0.5f), new(0f, 0f, 1f, 1f), new(0f, 0f)),
    ];

    // csharpier-ignore
    List<int> Indices { get; } =
    [
        0, 1, 2, // top right part
        2, 3, 0, // bottom left part

        4, 5, 6,
        6, 7, 4,

        8, 9, 10,
        10, 11, 8,

        12, 13, 14,
        14, 15, 12,

        16, 17, 18,
        18, 19, 16,

        20, 21, 22,
        22, 23, 20
    ];

    public HelloSquare()
    {
        KeyDownEvent.AddClassHandler<TopLevel>((_, e) => KeyState = e, handledEventsToo: true);
        KeyUpEvent.AddClassHandler<TopLevel>((_, _) => KeyState = null, handledEventsToo: true);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        LastPointerPostion = e.GetPosition(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var position = e.GetPosition(this);
        var dX = position.X - LastPointerPostion.X;
        var dY = position.Y - LastPointerPostion.Y;
        PointerPostionDiff = new((float)dX, (float)dY);
        LastPointerPostion = position;
        Debug.WriteLine(PointerPostionDiff);
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var size = e.NewSize * scaling;
        ViewPortSize = new((int)size.Width, (int)size.Height);
        Camera?.SetSize(Bounds.Size);
    }

    protected override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
        
        Gl = GL.GetApi(gl.GetProcAddress);

        Vao = new(Gl);
        PositionVbo = new(Gl, Vertices.Select(v=>v.Position).ToArray());
        ColorVbo = new(Gl, Vertices.Select(v=>v.Color).ToArray());
        TexCoordVbo = new(Gl, Vertices.Select(v=>v.TexCoord).ToArray());
        Ebo = new(Gl, Indices);
        
        Vao.Link(0, 3, VertexAttribPointerType.Float, PositionVbo);
        Vao.Link(1, 4, VertexAttribPointerType.Float, ColorVbo);
        Vao.Link(2, 2, VertexAttribPointerType.Float, TexCoordVbo);

        Vao.Unbind();
        PositionVbo.Unbind();
        Ebo.Unbind();

        Shader = new(Gl, "simple");

        HappyTexture = new(Gl, "happy", 1);

        Camera = new(Bounds.Size, Vector3.Zero);

        gl.Enable(GL_DEPTH_TEST);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
        
        Vao?.Delete();
        PositionVbo?.Delete();
        ColorVbo?.Delete();
        TexCoordVbo?.Delete();
        Ebo?.Delete();
        
        Shader?.Delete();
        
        HappyTexture?.Delete();
        
        Gl?.Dispose();
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (Gl is null || Shader is null || Camera is null)
            return;

        gl.Viewport(0, 0, ViewPortSize.Width, ViewPortSize.Height);

        gl.ClearColor(0, 0, 0, 1);
        gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        // var position = Bounds.Center;
        // var scale = new Vector2(150, 100);
        // var rotation = MathF.Sin(St.Elapsed.Milliseconds) * MathF.PI;
        // var trans = Matrix4x4.CreateTranslation((float)position.X, (float)position.Y, 0);
        // var sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
        // var rot = Matrix4x4.CreateRotationZ(rotation);
        // Shader.SetMatrix(gl, "model", sca * rot * trans);
        // Shader.SetMatrix(gl, "model", sca * trans);

        var model = Matrix4x4.Identity;
        var view = Camera.GetViewMatrix();
        var projection = Camera.GetProjectionMatrix();

        var rotationX = Matrix4x4.CreateRotationX(RotationX);
        var rotationY = Matrix4x4.CreateRotationY(RotationY);
        var translation = Matrix4x4.CreateTranslation(0f, 0f, -3f);
        model = rotationX * rotationY * translation;
        // RotationX += 0.01f;
        RotationY += 0.01f;

        Shader.SetMatrix("model", model);
        Shader.SetMatrix("view", view);
        Shader.SetMatrix("projection", projection);

        Shader.SetTexture(HappyTexture);
        HappyTexture?.Bind();

        Shader.Bind();

        Vao?.Bind();
        Ebo?.Bind();
        // gl.DrawArrays(GL_TRIANGLES, 0, 3);
        Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);

        gl.BindVertexArray(0);
        gl.BindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        HappyTexture?.Unbind();
        
        // Shader.Unbind();

        // Dispatcher.UIThread.Post(RequestNextFrameRendering, DispatcherPriority.Background);
        OnFrameUpdate();
    }

    private void OnFrameUpdate()
    {
        Camera?.InputController(KeyState, PointerPostionDiff);
        PointerPostionDiff = Vector2.Zero;
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}
