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
using AvaloniaSilkOpengles.Assets.Models;
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
    Model? Model1 { get; set; }
    Model? Model2 { get; set; }
    Model? Crow { get; set; }
    Model? CrowOutline { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? SimpleShader { get; set; }
    ShaderHandler? OutlineShader { get; set; }
    Camera3D? Camera { get; set; }
    PixelSize ViewPortSize { get; set; }
    float RotationX { get; set; }
    float RotationY { get; set; }
    float RotationZ { get; set; }
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
        Model1 = new(Gl, new ModelRead("trees"));
        Model2 = new(Gl, new ModelRead("ground"));
        Crow = new(Gl, new ModelRead("crow"));
        CrowOutline = new(Gl, new ModelRead("crow-outline"));

        LightShader = new(Gl, "light");
        SimpleShader = new(Gl, "simple");
        OutlineShader = new(Gl, "outline");

        Camera = new(Bounds.Size, Vector3.Zero);
        // Camera = new(Bounds.Size, new(10, 10, 10));

        Gl.Enable(EnableCap.DepthTest);
        Gl.DepthFunc(DepthFunction.Less);

        Gl.Enable(EnableCap.StencilTest);
        Gl.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

        // Gl.FrontFace(FrontFaceDirection.CW);
        // Gl.Enable(EnableCap.CullFace);
        // Gl.CullFace(TriangleFace.Back);

        // Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        // Gl.Enable(EnableCap.Blend);
    }

    protected override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);

        LightCube?.Delete();
        Chunk?.Delete();

        SimpleShader?.Delete();
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
        if (Gl is null || SimpleShader is null || LightShader is null || Camera is null)
            return;

        CheckSettings(Gl);
        // gl.Viewport(0, 0, ViewPortSize.Width, ViewPortSize.Height);

        // Gl.ClearColor(0, 0, 0, 1);
        var backGround = new Vector4(0.85f, 0.85f, 0.90f, 1.0f);
        Gl.ClearColor(backGround.X, backGround.Y, backGround.Z, backGround.W);
        Gl.Clear(
            ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit
                | ClearBufferMask.StencilBufferBit
        );

        // var position = Bounds.Center;
        // var scale = new Vector2(150, 100);
        // var rotation = MathF.Sin(St.Elapsed.Milliseconds) * MathF.PI;
        // var trans = Matrix4x4.CreateTranslation((float)position.X, (float)position.Y, 0);
        // var sca = Matrix4x4.CreateScale(scale.X, scale.Y, 1);
        // var rot = Matrix4x4.CreateRotationZ(rotation);
        // Shader.SetMatrix(gl, "model", sca * rot * trans);
        // Shader.SetMatrix(gl, "model", sca * trans);

        // var model = Matrix4x4.Identity;
        // var view = Camera.GetViewMatrix();
        // var projection = Camera.GetProjectionMatrix();
        var cameraMatrix = Camera.GetMatrix();

        // var lightColor = new Vector4(1.0f, 0.8f, 0.6f, 1.0f);
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        // var lightPos = new Vector3(0.5f, 0.5f, 0.5f);
        var lightPos = new Vector3(3.0f, 4.0f, -3.0f);
        // var lightPos = new Vector3(10f, 8f, 5f);
        // var lightPos = new Vector3(-8f, 25f, -15f);
        // var lightModel =
        //     Matrix4x4.CreateScale(new Vector3(0.05f, 0.05f, 0.05f))
        //     * Matrix4x4.CreateTranslation(lightPos);
        var scale = new Vector3(0.05f, 0.05f, 0.05f);

        LightShader.Use();
        // LightShader?.SetMatrix("view", view);
        // LightShader?.SetMatrix("projection", projection);
        // LightShader?.SetMatrix("camMatrix", cameraMatrix);
        // LightShader.SetMatrix("model", lightModel);
        LightShader.SetVector4("lightColor", lightColor);

        LightCube?.Render(
            LightShader,
            Camera,
            scale,
            Quaternion.Identity,
            lightPos,
            Matrix4x4.Identity
        );

        var rotationX = Matrix4x4.CreateRotationX(RotationX);
        var rotationY = Matrix4x4.CreateRotationY(RotationY);
        var rotationZ = Matrix4x4.CreateRotationZ(RotationZ);
        var rotation = Quaternion.CreateFromRotationMatrix(rotationX * rotationY * rotationZ);
        var translation = new Vector3(0f, 0f, 0f);
        // var model = rotationX * rotationY * translation;
        // RotationX += 0.001f;
        // RotationY += 0.001f;
        // RotationZ += 0.001f;

        SimpleShader.Use();
        // ChunkShader.SetMatrix("model", model);
        // ChunkShader.SetMatrix("view", view);
        // ChunkShader.SetMatrix("projection", projection);
        // ChunkShader.SetMatrix("camMatrix", cameraMatrix);
        SimpleShader.SetVector4("lightColor", lightColor);
        SimpleShader.SetVector3("lightPos", lightPos);
        SimpleShader.SetVector4("backGround", backGround);
        // ChunkShader.SetVector3("camPos", Camera.Position);

        Gl.StencilFunc(StencilFunction.Always, 1, 0XFF);
        Gl.StencilMask(0xFF);

        // ChunkShader.SetVector3("light_direct", new(0, 0, 1));
        // Chunk?.Render(ChunkShader, Camera, Vector3.One, rotation, translation, Matrix4x4.Identity);
        // Model1?.Render(SimpleShader, Camera);
        // Model2?.Render(SimpleShader, Camera);
        Crow?.Render(SimpleShader, Camera);

        Gl.StencilFunc(StencilFunction.Notequal, 1, 0XFF);
        Gl.StencilMask(0x00);
        Gl.Disable(EnableCap.DepthTest);

        OutlineShader?.Use();
        OutlineShader?.SetValue("outlining", 0.08f);
        CrowOutline?.Render(OutlineShader, Camera);

        Gl.StencilMask(0xFF);
        Gl.StencilFunc(StencilFunction.Always, 0, 0XFF);
        Gl.Enable(EnableCap.DepthTest);

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
