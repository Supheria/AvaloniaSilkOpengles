using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.Sphere;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Controls;

public class HelloSphere : SilkNetOpenGlControl
{
    SelectableSphere Sun { get; } = new();
    List<SelectableSphere> Spheres { get; } = [];
    ShaderHandler? LightShader { get; set; }
    ShaderHandler? SphereShader { get; set; }
    Texture2D? TextureWood { get; set; }
    Texture2D? TextureMoon { get; set; }
    bool DoDrag { get; set; }

    // private Texture2D LoadTexture(GL gl, string name)
    // {
    //     using var stream = TextureRead.Read(name);
    //     return new(gl, stream, TextureType.Diffuse, 0);
    // }

    protected override void OnGlInit(GL gl)
    {
        TextureWood = Texture2D.Create(gl, "wood", 0, TextureType.Diffuse);
        TextureMoon = Texture2D.Create(gl, "moon", 0, TextureType.Diffuse);

        var sphereModel = new IcoSphereModel(gl, 3);
        Sun.SetCurrentModel(gl, sphereModel);
        for (var x = 0f; x < 5; x += 0.5f)
        {
            for (var y = 0f; y < 3; y += 0.5f)
            {
                var sphere = new SelectableSphere { Position = new(x, y, -5f), Scale = new(0.2f) };
                sphere.SetCurrentModel(gl, sphereModel);
                sphere.SetTextures(TextureWood);
                Spheres.Add(sphere);
            }
        }

        LightShader = ShaderHandler.Create(gl, "light", []);
        SphereShader = ShaderHandler.Create(gl, "sphere", []);

        gl.Enable(EnableCap.DepthTest);

        // gl.FrontFace(FrontFaceDirection.Ccw);
        // gl.Enable(EnableCap.CullFace);
        // gl.CullFace(TriangleFace.Back);
    }

    protected override void OnGlDeinit(GL gl)
    {
        Sun.Delete(gl);
        foreach (var sphere in Spheres)
            sphere.Delete(gl);
        SphereShader?.Delete(gl);
    }

    protected override void OnGlRender(GL gl)
    {
        if (LightShader is null || SphereShader is null)
            return;

        var color = new RenderColor(Colors.SkyBlue);
        gl.ClearColor(color.R, color.G, color.B, 1);
        gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var sphere in Spheres)
        {
            sphere.RotationDegrees += new Vector3(0.0f, 0.01f, 0);
        }

        var lightPos = new Vector3(1, 1, 1);
        var lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        LightShader.Use(gl);
        LightShader.UniformVector4(gl, "lightColor", lightColor);
        Sun.Scale = new(0.05f, 0.05f, 0.05f);
        Sun.Position = lightPos;
        Sun.Render(gl, LightShader, Camera);

        SphereShader.Use(gl);
        SphereShader.UniformVector4(gl, "lightColor", lightColor);
        SphereShader.UniformVector3(gl, "lightPos", lightPos);
        foreach (var sphere in Spheres)
            sphere.Render(gl, SphereShader, Camera);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        DoDrag = true;
        var position = e.GetPosition(this);
        LastPointerPostion = position;
        PickObjectOnScreen((float)position.X, (float)position.Y);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (DoDrag)
            base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        DoDrag = false;
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
        var projection = Camera.Project;
        Matrix4.Invert(ref projection, out var projectionInvert);
        var rayEye = Vector4.Transform(rayClip, projectionInvert);
        rayEye = new Vector4(rayEye.X, rayEye.Y, -1f, 0f);

        // 4D world coordinates
        var view = Camera.View;
        Matrix4.Invert(ref view, out var viewInvert);
        var r = Vector4.Transform(rayEye, viewInvert);
        var ray = Vector3.Normalize(new(r.X, r.Y, r.Z));
        FindClosestAsteroidHitByRay(ray);
    }

    private void FindClosestAsteroidHitByRay(Vector3 rayWorldCoordinates)
    {
        SelectableSphere? bestCandidate = null;
        float? bestDistance = null;
        foreach (var gameObject in Spheres)
        {
            var candidateDistance = gameObject.IntersectsRay(rayWorldCoordinates, Camera.Position);
            if (!candidateDistance.HasValue)
                continue;
            if (!bestDistance.HasValue)
            {
                if (candidateDistance < 1.1f)
                    continue;
                bestDistance = candidateDistance;
                bestCandidate = gameObject;
                continue;
            }
            if (candidateDistance < bestDistance)
            {
                bestDistance = candidateDistance;
                bestCandidate = gameObject;
            }
        }
        if (bestCandidate != null)
        {
            if (bestCandidate.Selected)
                bestCandidate.SetTextures(TextureWood);
            else
                bestCandidate.SetTextures(TextureMoon);
            bestCandidate.Selected = !bestCandidate.Selected;
        }
    }
}
