using System;
using System.Numerics;
using Avalonia.Input;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World2;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class TerrainLoader : SilkNetOpenGlControl
{
    bool DoDrag { get; set; }
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
        
        var scale = new Vector3(0.1f, 0.1f, 0.1f);
        TestTerrain.Scale = scale;
        
        TerrainShader.Use();
        TestTerrain.Render(TerrainShader, Camera);
    }

    protected override void OnPointerEntered(PointerEventArgs e) { }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        DoDrag = true;
        LastPointerPostion = e.GetPosition(this);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (DoDrag)
            base.OnPointerMoved(e);
        // var position = e.GetPosition(this);
        // var size = Bounds.Size;
        //
        // var ndcX = position.X * (2 / size.Width) - 1;
        // var ndcY = 1 - position.Y * (2 / size.Height);
        //
        // var nearClipPlaneHeight = MathF.Tan(float.DegreesToRadians(FovDegrees) / 2) * NearClipPlane;
        // var aspectRatio = size.Width / size.Height;
        //
        // var x3D = ndcX * nearClipPlaneHeight * aspectRatio;
        // var y3D = ndcY * nearClipPlaneHeight;
        //
        // var nearClipPlanePoint = new Vector3((float)x3D, (float)y3D, -NearClipPlane);
        // if ( Matrix4x4.Invert(Camera.ViewMatrix, out var invertViewMatrix))
        // nearClipPlanePoint =
        // // var rayNormalizedDeviceCoordinates = new Vector3((float)x, (float)y, z);
        // // var rayClip = new Vector4(rayNormalizedDeviceCoordinates.X, rayNormalizedDeviceCoordinates.Y, -1f, 1f);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        DoDrag = false;
    }
}
