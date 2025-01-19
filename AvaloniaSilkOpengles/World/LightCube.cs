using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class LightCube
{
    GL Gl { get; }
    List<VertexSingle> Vertices { get; } = [];
    List<uint> Indices { get; } = [];
    uint IndexHead { get; set; }
    public Vector3 Position { get; set; }
    VaoHandler? Vao { get; set; }
    VboHandler<VertexSingle>? VertexVbo { get; set; }
    IboHandler? Ibo { get; set; }

    public LightCube(GL gl, Vector3 position)
    {
        Gl = gl;
        Position = position;
        GenerateFaces();
        Build();
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
        var vertices = VertexData.Coords[face].Select(v => new VertexSingle(v + Position));
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

    private void Build()
    {
        Vao = new(Gl);
        VertexVbo = new(Gl, Vertices);
        Ibo = new(Gl, Indices);

        Vao.Link(VertexVbo, 0, VertexElement.Position);
    }

    public unsafe void Render()
    {
        Vao?.Bind();
        Ibo?.Bind();

        Gl.DrawElements(
            PrimitiveType.Triangles,
            (uint)Indices.Count,
            DrawElementsType.UnsignedInt,
            null
        );

        // shader.Unbind();
        Vao?.Unbind();
        Ibo?.Unbind();
    }

    public void Delete()
    {
        Vao?.Delete();
        VertexVbo?.Delete();
        Ibo?.Delete();
    }
}
