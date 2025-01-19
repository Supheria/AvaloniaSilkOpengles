namespace AvaloniaSilkOpengles.Graphics;

public record struct VertexElement(int StartPointer, int Count)
{
    public static VertexElement Position {get;} = new(0, 3);
    public static VertexElement Normal {get;} = new(sizeof(float) * 3, 3);
    public static VertexElement Uv {get;} = new(sizeof(float) * 6, 2);
    public static VertexElement Color {get;} = new(sizeof(float) * 6, 4);
}