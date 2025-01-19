using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public readonly struct Vertex
{
    public Vector3 Coord { get; }
    public Vector3 Normal { get; }
    public Vector3 Color { get; }
    public Vector2 Uv { get; }

    public Vertex(Vector3 coord, Vector3 normal, Vector3 color, Vector2 uv)
    {
        Coord = coord;
        Normal = normal;
        Color = color;
        Uv = uv;
    }

    public Vertex(Vector3 coord)
    {
        Coord = coord;
        Normal = Vector3.Zero;
        Color = Vector3.Zero;
        Uv = Vector2.Zero;
    }
    
    public Vertex(Vector3 coord, Vector3 color)
    {
        Coord = coord;
        Normal = Vector3.Zero;
        Color = color;
        Uv = Vector2.Zero;
    }

    public Vertex(Vector3 coord, Vector3 normal, Vector3 color)
    {
        Coord = coord;
        Normal = normal;
        Color = color;
        Uv = Vector2.Zero;
    }

    public Vertex(Vector3 coord, Vector3 normal, Vector2 uv)
    {
        Coord = coord;
        Normal = normal;
        Color = Vector3.Zero;
        Uv = uv;
    }
}
