using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class RenderableObject
{
    protected List<Vertex> Vertices { get; } = [];
    protected List<uint> Indices { get; } = [];
    protected Mesh? Mesh { get; init;}
    
    public void Render(ShaderHandler shader, Camera3D camera, Vector3 scale, Quaternion rotation, Vector3 translation, Matrix4x4 matrix)
    {
        Mesh?.Render(shader, camera, scale, rotation, translation, matrix);
    }
    
    public void Delete()
    {
        Mesh?.Delete();
    }
}
