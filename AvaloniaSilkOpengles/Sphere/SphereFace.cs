using System.Drawing;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.Sphere;

public class SphereFace : Face
{
    public Position P1 { get; }
    public Position P2 { get; }
    public Position P3 { get; }

    public SphereFace(Position p1, Position p2, Position p3)
    {
        P1 = p1;
        P2 = p2;
        P3 = p3;
    }
    
    public void GenerateVertices(TexUv uv1, TexUv uv2, TexUv uv3)
    {
        var normal = GetNormal();
        RepairColorStrip(ref uv1, ref uv2, ref uv3);
        AddVertex(P1, normal, RenderColor.Zero, uv1);
        AddVertex(P2, normal, RenderColor.Zero, uv2);
        AddVertex(P3, normal, RenderColor.Zero, uv3);
    }

    private Normal GetNormal()
    {
        var u = P3.Value - P2.Value;
        var v = P1.Value - P2.Value;
        var uxvX = u.Y * v.Z - u.Z * v.Y;
        var uxvY = u.Z * v.X - u.X * v.Z;
        var uxvZ = u.X * v.Y - u.Y * v.X;
        return new(uxvX, uxvY, uxvZ);
    }

    private static void RepairColorStrip(ref TexUv uv1, ref TexUv uv2, ref TexUv uv3)
    {
        if (uv1.X - uv2.X >= 0.8f)
            uv1 = new(uv1.X - 1, uv1.Y);
        if (uv2.X - uv3.X >= 0.8f)
            uv2 = new(uv2.X - 1, uv2.Y);
        if (uv3.X - uv1.X >= 0.8f)
            uv3 = new(uv3.X - 1, uv3.Y);
        // twice
        if (uv1.X - uv2.X >= 0.8f)
            uv1 = new(uv1.X - 1, uv1.Y);
        if (uv2.X - uv3.X >= 0.8f)
            uv2 = new(uv2.X - 1, uv2.Y);
        if (uv3.X - uv1.X >= 0.8f)
            uv3 = new(uv3.X - 1, uv3.Y);
    }
}
