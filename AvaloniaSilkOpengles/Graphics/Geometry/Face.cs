using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class Face
{
    List<Position> Positions { get; } = new();
    List<Normal> Normals { get; } = new();
    List<RenderColor> Color { get; } = new();
    List<TexUv> Uv { get; } = new();
    
    protected void AddVertex(Position position, Normal normal, RenderColor color, TexUv uv)
    {
        Positions.Add(position);
        Normals.Add(normal);
        Color.Add(color);
        Uv.Add(uv);
    }

    public List<Vertex> GetVertices()
    {
        var vertices = new List<Vertex>();
        for (var i = 0; i < Positions.Count; i++)
        {
            var position = Positions[i];
            var normal = Normals[i];
            var color = Color[i];
            var uv = Uv[i];
            vertices.Add(new Vertex
            {
                Position = position,
                Normal = normal,
                Color = color,
                Uv = uv
            });
        }
        return vertices;
    }
}
