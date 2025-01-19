using System.Collections.Generic;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class RenderableObject
{
    protected List<Vertex> Vertices { get; } = [];
    protected List<uint> Indices { get; } = [];
    protected Mesh? Mesh { get; init;}
    
    public void Render(ShaderHandler shader, Camera3D camera)
    {
        Mesh?.Render(shader, camera);
    }
    
    public void Delete()
    {
        Mesh?.Delete();
    }
}
