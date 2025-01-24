using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class RenderableObject
{
    GL Gl { get; }
    Mesh Mesh { get; set; } = new();
    List<Vertex> Vertices { get; } = [];
    List<uint> Indices { get; } = [];
    protected List<Texture2DHandler> Textures { get; } = [];
    public Vector3 Scale
    {
        get => Mesh.Scale;
        set => Mesh.Scale = value;
    }
    public Vector3 RotationDegrees
    {
        get => _rotationDegrees;
        set
        {
            _rotationDegrees = value;
            var rX = Matrix4x4.CreateRotationX(_rotationDegrees.X);
            var rY = Matrix4x4.CreateRotationY(_rotationDegrees.Y);
            var rZ = Matrix4x4.CreateRotationZ(_rotationDegrees.Z);
            var matrix = rX * rY * rZ;
            Mesh.Rotation = Quaternion.CreateFromRotationMatrix(matrix);
        }
    }
    Vector3 _rotationDegrees;
    public Vector3 Translation
    {
        get => Mesh.Translation;
        set => Mesh.Translation = value;
    }
    public Matrix4x4 Matrix
    {
        get => Mesh.Matrix;
        set => Mesh.Matrix = value;
    }
    public PrimitiveType RenderMode
    {
        get => Mesh.RenderMode;
        set => Mesh.RenderMode = value;
    }

    protected RenderableObject(GL gl)
    {
        Gl = gl;
    }

    public void Render(ShaderHandler shader, Camera3D camera)
    {
        Mesh.Render(shader, camera);
    }

    public void Delete()
    {
        Mesh.Delete();
    }

    protected void CreateMesh()
    {
        Mesh.Create(Gl, Vertices, Indices, Textures);
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
