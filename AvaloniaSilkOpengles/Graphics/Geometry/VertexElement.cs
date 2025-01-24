namespace AvaloniaSilkOpengles.Graphics;

public record struct VertexElement(uint Plot, int Count, int StartPointer)
{
    public static VertexElement Position {get;} = new(0, 3, 0);
    public static VertexElement Normal {get;} = new(1, 3, sizeof(float) * 3);
    public static VertexElement Color {get;} = new(2, 3, sizeof(float) * 6);
    public static VertexElement Uv {get;} = new(3, 2, sizeof(float) * 9);
}