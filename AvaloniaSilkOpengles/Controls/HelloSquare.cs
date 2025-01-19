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
using Vector = Avalonia.Vector;

namespace AvaloniaSilkOpengles.Controls;

public unsafe class HelloSquare : OpenGlControlBase, ICustomHitTest
{
    GL? Gl { get; set; }
    LightCube? LightCube { get; set; }
    Chunk? Chunk { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? ChunkShader { get; set; }
    Camera3D? Camera { get; set; }
    PixelSize ViewPortSize { get; set; }
    float RotationX { get; set; }
    float RotationY { get; set; }
    KeyEventArgs? KeyState { get; set; }
    Vector2 PointerPostionDiff { get; set; }
    Point LastPointerPostion { get; set; }
    bool Alpha { get; set; }

    public HelloSquare()
    {
        KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, handledEventsToo: true);
        KeyUpEvent.AddClassHandler<TopLevel>((_, _) => KeyState = null, handledEventsToo: true);
    }

    private void OnKeyDown(TopLevel _, KeyEventArgs e)
    {
        KeyState = e;
        switch (e.Key)
        {
            case Key.B:
                Alpha = !Alpha;
                break;
        }
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

        LightCube = new(Gl, new());
        Chunk = new(Gl, new());

        LightShader = new(Gl, "light");
        ChunkShader = new(Gl, "simple");

        // Camera = new(Bounds.Size, Vector3.Zero);
        Camera = new(Bounds.Size, new(10, 10, 10));

        Gl.Enable(EnableCap.DepthTest);

        // Gl.FrontFace(FrontFaceDirection.CW);
        Gl.Enable(EnableCap.CullFace);
        Gl.CullFace(TriangleFace.Back);

        // Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        // Gl.Enable(EnableCap.Blend);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);

        LightCube?.Delete();
        Chunk?.Delete();

        ChunkShader?.Delete();
        LightShader?.Delete();

        Gl?.Dispose();
    }

    private void CheckSettings(GL gl)
    {
        gl.Viewport(0, 0, (uint)ViewPortSize.Width, (uint)ViewPortSize.Height);
        // if (Alpha)
        // {
        //     gl.Enable(EnableCap.Blend);
        //     // gl.Disable(EnableCap.DepthTest);
        //     // gl.Disable(EnableCap.CullFace);
        // }
        // else
        // {
        //     gl.Disable(EnableCap.Blend);
        //     // gl.Enable(EnableCap.DepthTest);
        //     // gl.Enable(EnableCap.CullFace);
        // }
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (Gl is null || ChunkShader is null || Camera is null)
            return;

        CheckSettings(Gl);
        // gl.Viewport(0, 0, ViewPortSize.Width, ViewPortSize.Height);

        Gl.ClearColor(0, 0, 0, 1);
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // var position = Bounds.Center;
        // var scale = new Vector2(150, 100);
        // var rotation = MathF.Sin(St.Elapsed.Milliseconds) * MathF.PI;
        // var trans = Matrix4x4.CreateTranslation((float)position.X, (float)position.Y, 0);
        // var sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
        // var rot = Matrix4x4.CreateRotationZ(rotation);
        // Shader.SetMatrix(gl, "model", sca * rot * trans);
        // Shader.SetMatrix(gl, "model", sca * trans);

        // var model = Matrix4x4.Identity;
        var view = Camera.GetViewMatrix();
        var projection = Camera.GetProjectionMatrix();
        
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        // var lightPos = new Vector3(0.5f, 0.5f, 0.5f);
        var lightPos = new Vector3(10f, 8f, 5f);
        var lightModel = Matrix4x4.CreateScale(new Vector3(0.05f, 0.05f, 0.05f)) * Matrix4x4.CreateTranslation(lightPos);

        LightShader?.Bind();
        LightShader?.SetMatrix("view", view);
        LightShader?.SetMatrix("projection", projection);
        LightShader?.SetMatrix("model", lightModel);
        LightShader?.SetVector4("lightColor", lightColor);

        LightCube?.Render();

        var rotationX = Matrix4x4.CreateRotationX(RotationX);
        var rotationY = Matrix4x4.CreateRotationY(RotationY);
        var translation = Matrix4x4.CreateTranslation(0f, 0f, 0f);
        var model = rotationX * rotationY * translation;
        // RotationX += 0.005f;
        // RotationY += 0.005f;

        ChunkShader.Bind();
        ChunkShader.SetMatrix("model", model);
        ChunkShader.SetMatrix("view", view);
        ChunkShader.SetMatrix("projection", projection);
        ChunkShader.SetVector4("lightColor", lightColor);
        ChunkShader.SetVector3("lightPos", lightPos);
        ChunkShader.SetVector3("camPos", Camera.Position);

        // ChunkShader.SetVector3("light_direct", new(0, 0, 1));
        Chunk?.Render(ChunkShader);

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
