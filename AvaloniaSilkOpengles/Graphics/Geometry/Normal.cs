using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public readonly record struct Normal(Vector3 Value)
{
    public static Normal Zero => new();

    public Normal()
        : this(Vector3.Zero) { }

    public Normal(float x, float y, float z)
        : this(new(x, y, z)) { }
}
