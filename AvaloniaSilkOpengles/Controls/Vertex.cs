using System.Numerics;

namespace AvaloniaSilkOpengles.Controls;

public readonly struct Vertex
{
    public readonly Vector3 Position;
    public readonly Vector4 Color;
    public readonly Vector2 TexCoord;
    public Vertex(Vector3 position, Vector4 color, Vector2 texCoord)
    {
        Position = position;
        Color = color;
        TexCoord = texCoord;
    }
}