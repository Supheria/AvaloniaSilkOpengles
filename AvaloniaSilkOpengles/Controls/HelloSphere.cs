using System.Drawing;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.Sphere;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class HelloSphere : SilkNetOpenGlControl
{
    SelectableSphere? Sun { get; set; }
    SelectableSphere? Sphere { get; set; }
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? SphereShader { get; set; }

    protected override void OnGlInit(GL gl)
    {
        Sun = new(gl, 3);
        Sphere = new(gl, 3);

        LightShader = new(gl, "light");
        SphereShader = new(gl, "sphere");

        gl.Enable(EnableCap.DepthTest);

        gl.FrontFace(FrontFaceDirection.Ccw);
        gl.Enable(EnableCap.CullFace);
        gl.CullFace(TriangleFace.Back);
    }

    protected override void OnGlDeinit(GL gl)
    {
        Sphere?.Delete(gl);
        SphereShader?.Delete(gl);
    }

    protected override void OnGlRender(GL gl)
    {
        if (Sun is null || Sphere is null || LightShader is null || SphereShader is null)
            return;

        var color = new RenderColor(Color.SkyBlue);
        gl.ClearColor(color.R, color.G, color.B, 1);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        var lightPos = new Vector3(1, 1, 1);
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        LightShader.Use(gl);
        LightShader.SetVector4(gl, "lightColor", lightColor);
        Sun.Scale = new(0.05f, 0.05f, 0.05f);
        Sun.Position = lightPos;
        Sun.Render(gl, LightShader, Camera);

        SphereShader.Use(gl);
        SphereShader.SetVector4(gl, "lightColor", lightColor);
        SphereShader.SetVector3(gl, "lightPos", lightPos);
        Sphere.Render(gl, SphereShader, Camera);
    }

    private void PickObjectOnScreen(float mouseX, float mouseY)
    {
        var width = (float)Bounds.Width;
        var height = (float)Bounds.Height;
        // heavily influenced by: http://antongerdelan.net/opengl/raycasting.html
        // viewport coordinate system
        // normalized device coordinates
        var x = (2f * mouseX) / width - 1f;
        var y = 1f - (2f * mouseY) / height;
        var z = 1f;
        var rayNormalizedDeviceCoordinates = new Vector3(x, y, z);

        // 4D homogeneous clip coordinates
        var rayClip = new Vector4(
            rayNormalizedDeviceCoordinates.X,
            rayNormalizedDeviceCoordinates.Y,
            -1f,
            1f
        );

        // 4D eye (camera) coordinates
        if (!Matrix4x4.Invert(Camera.ProjectionMatrix, out var projectionInvert))
            return;
        var rayEye = Vector4.Transform(rayClip, projectionInvert);
        rayEye = new Vector4(rayEye.X, rayEye.Y, -1f, 0f);

        // 4D world coordinates
        if (!Matrix4x4.Invert(Camera.ViewMatrix, out var viewInvert))
            return;
        var r = Vector4.Transform(rayEye, viewInvert);
        var ray = Vector3.Normalize(new(r.X, r.Y, r.Z));
        // FindClosestAsteroidHitByRay(r);
    }
}
