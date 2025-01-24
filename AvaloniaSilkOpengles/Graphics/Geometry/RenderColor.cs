using System.Drawing;
using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public readonly record struct RenderColor(Vector3 Value)
{
    public float R => Value.X;
    public float G => Value.Y;
    public float B => Value.Z;
    /// <summary>
    /// 1, 1, 1
    /// </summary>
    public static RenderColor Zero => new();

    public RenderColor(Color color)
        : this(Normalize(color)) { }

    public RenderColor()
        : this(Color.Transparent) { }

    private static Vector3 Normalize(Color color)
    {
        return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
    }
}
