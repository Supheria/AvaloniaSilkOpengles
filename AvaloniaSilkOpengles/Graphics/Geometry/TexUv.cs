using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public readonly record struct TexUv(Vector2 Value)
{
    public float X => Value.X;
    public float Y => Value.Y;
    public static TexUv Zero => new();

    public TexUv()
        : this(Vector2.Zero) { }

    public TexUv(float x, float y)
        : this(new(x, y)) { }
}
