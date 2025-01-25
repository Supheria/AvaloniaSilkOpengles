using System.Collections.Generic;
using AvaloniaSilkOpengles.Graphics.Resources;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class RenderableObject
{
    Mesh Mesh { get; set; } = new();
    List<Vertex> Vertices { get; } = [];
    List<uint> Indices { get; } = [];

    public void Render(
        GL gl,
        PrimitiveType renderMode,
        Vector3 scale,
        Quaternion rotation,
        Vector3 translation,
        Matrix4 matrix,
        List<Texture2DHandler> textures,
        ShaderHandler shader,
        Camera3D camera
    )
    {
        Mesh.Render(gl, renderMode, scale, rotation, translation, matrix, textures, shader, camera);
    }

    public void Delete(GL gl)
    {
        Mesh.Delete(gl);
    }

    protected void CreateMesh(GL gl)
    {
        Mesh.Create(gl, Vertices, Indices);
    }

    protected void AddTriangleIndices(uint v1, uint v2, uint v3)
    {
        Indices.AddRange([v1, v2, v3]);
    }

    protected void AddVertex(Position position, Normal normal, RenderColor color, TexUv uv)
    {
        Vertices.Add(
            new()
            {
                Position = position,
                Normal = normal,
                Color = color,
                Uv = uv,
            }
        );
    }

    protected void AddVertices(Face face)
    {
        Vertices.AddRange(face.GetVertices());
    }
}
