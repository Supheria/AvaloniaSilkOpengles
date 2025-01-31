using Microsoft.Xna.Framework;

namespace AvaloniaSilkOpengles.Graphics;

public readonly record struct Position(Vector3 Value)
{
    public float X => Value.X;
    public float Y => Value.Y;
    public float Z => Value.Z;
    public static Position Zero => new();

    public Position()
        : this(Vector3.Zero) { }

    public Position(float x, float y, float z)
        : this(new(x, y, z)) { }

    public static Position Normalize(float x, float y, float z)
    {
        return new(Vector3.Normalize(new(x, y, z)));
    }
}
