using System;
using System.Numerics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;
using AvaloniaSilkOpengles.Graphics;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public abstract class SilkNetOpenGlControl : OpenGlControlBase, ICustomHitTest
{
    public EventHandler<FrameInfo>? FrameInfoUpdated;
    GL? Gl { get; set; }
    protected Camera3D Camera { get; } = new(Vector3.Zero);
    protected float FovDegrees { get; private set; } = 45.0f;
    protected float NearClipPlane { get; private set; } = 0.1f;
    protected float FarClipPlane { get; private set; } = 75.0f;
    PixelSize ViewPortSize { get; set; }
    bool DoChangeViewPort { get; set; }
    DispatcherTimer Timer { get; } = new();
    DateTime LastFrameUpdateTime { get; set; }
    DateTime LastFrameInfoUpdateTime { get; set; }
    int FrameCount { get; set; }
    KeyEventArgs? KeyState { get; set; }
    Vector2 PointerPostionDiff { get; set; }
    protected Point LastPointerPostion { get; set; }

    public SilkNetOpenGlControl()
    {
        KeyDownEvent.AddClassHandler<TopLevel>((_, e) => KeyState = e, handledEventsToo: true);
        KeyUpEvent.AddClassHandler<TopLevel>((_, _) => KeyState = null, handledEventsToo: true);
        Timer.Interval = TimeSpan.FromMilliseconds(10);
        Timer.Tick += (_, _) => RequestNextFrameRendering();
        Timer.Start();
    }

    public bool HitTest(Point point)
    {
        return true;
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
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var size = e.NewSize * scaling;
        var viewPortSize = new PixelSize((int)size.Width, (int)size.Height);
        if (ViewPortSize != viewPortSize)
        {
            DoChangeViewPort = true;
            ViewPortSize = viewPortSize;
        }
        SetCameraProjection(FovDegrees, NearClipPlane, FarClipPlane);
    }
    
    protected void SetCameraProjection(float fovDegrees, float nearClipPlane, float farClipPlane)
    {
        FovDegrees = fovDegrees;
        NearClipPlane = nearClipPlane;
        FarClipPlane = farClipPlane;
        Camera.SetSize(Bounds.Size, fovDegrees, nearClipPlane, farClipPlane);
    }

    protected sealed override void OnOpenGlInit(GlInterface gl)
    {
        base.OnOpenGlInit(gl);
        Gl = GL.GetApi(gl.GetProcAddress);
        OnGlInit(Gl);
    }

    protected abstract void OnGlInit(GL gl);

    protected sealed override void OnOpenGlDeinit(GlInterface gl)
    {
        base.OnOpenGlDeinit(gl);
        OnGlDeinit();
        Gl?.Dispose();
    }

    protected abstract void OnGlDeinit();

    protected sealed override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (Gl is null)
            return;
        CheckSettings(gl);
        OnGlRender(Gl);
        OnFrameUpdate();
    }

    private void CheckSettings(GlInterface gl)
    {
        if (DoChangeViewPort)
        {
            gl.Viewport(0, 0, ViewPortSize.Width, ViewPortSize.Height);
            DoChangeViewPort = false;
        }
    }

    protected abstract void OnGlRender(GL gl);

    private void OnFrameUpdate()
    {
        FrameCount++;
        var now = DateTime.Now;
        var timeDelta = (now - LastFrameUpdateTime).TotalMilliseconds / 1000;
        Camera.UpdateControl(KeyState, PointerPostionDiff, (float)timeDelta);
        PointerPostionDiff = Vector2.Zero;
        LastFrameUpdateTime = now;

        timeDelta = (now - LastFrameInfoUpdateTime).TotalMilliseconds;
        if (!(timeDelta > 500))
            return;
        var fps = FrameCount * 1000 / timeDelta;
        var mspf = timeDelta / FrameCount;
        var frameInfo = new FrameInfo(timeDelta, fps, mspf);
        FrameInfoUpdated?.Invoke(this, frameInfo);
        LastFrameInfoUpdateTime = now;
        FrameCount = 0;
    }
}
