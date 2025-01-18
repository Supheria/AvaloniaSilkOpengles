using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class LightCube
{
    GL Gl { get; }
    List<Vector3> Vertices { get; } = [];
    List<uint> Indices { get; } = [];
    uint IndexHead { get; set; }
    public Vector3 Position { get; set; }
    VaoHandler? Vao { get; set; }
    VboHandler<Vector3>? VertexVbo { get; set; }
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
        var vertices = FaceData.VertexData[face].Select(v => v + Position);
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

        Vao.Link(VertexVbo, 0, 3);
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
