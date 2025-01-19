using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class LightCube : RenderableObject
{
    uint IndexHead { get; set; }
    public Vector3 Position { get; set; }

    public LightCube(GL gl, Vector3 position)
    {
        Position = position;
        GenerateFaces();
        Mesh = new(gl, Vertices, Indices, []);
    }

    private void GenerateFaces()
    {
        AddFace(Face.Front);
        AddFace(Face.Right);
        AddFace(Face.Back);
        AddFace(Face.Left);
        AddFace(Face.Top);
        AddFace(Face.Bottom);
    }

    private void AddFace(Face face)
    {
        var coords = VertexData.Coords[face].Select(c => c + Position);
        var vertices = new List<Vertex>();
        switch (face)
        {
            case Face.Front:
                vertices.AddRange(coords.Select(c => new Vertex(c, new(1.0f, 0.0f, 0.0f))));
                break;
            case Face.Right:
                vertices.AddRange(coords.Select(c => new Vertex(c, new(0.0f, 1.0f, 0.0f))));
                break;
            case Face.Top:
                vertices.AddRange(coords.Select(c => new Vertex(c, new(0.0f, 0.0f, 1.0f))));
                break;
            default:
                vertices.AddRange(coords.Select(c => new Vertex(c, new(1.0f, 1.0f, 1.0f))));
                break;
        }
        Vertices.AddRange(vertices);
        Indices.AddRange(
            [
                0 + IndexHead,
                1 + IndexHead,
                2 + IndexHead,
                2 + IndexHead,
                3 + IndexHead,
                0 + IndexHead,
            ]
        );
        IndexHead += 4;
    }
}
