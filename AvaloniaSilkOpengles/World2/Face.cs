using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using LocalUtilities.General;

namespace AvaloniaSilkOpengles.World2;

public sealed class Face
{
    public List<Vertex> Vertices { get; }

    public Face(IList<Vector3> coords, Vector3 color)
    {
        Vertices = new(coords.Count);
        var normal = GetNormal(coords);
        foreach (var coord in coords)
        {
            var vertex = new Vertex(coord, normal, color);
            Vertices.Add(vertex);
        }
    }

    private Vector3 GetNormal(IList<Vector3> coords)
    {
        var u = coords[2] - coords[1];
        var v = coords[0] - coords[1];
        var uxvX = u.Y * v.Z - u.Z * v.Y;
        var uxvY = u.Z * v.X - u.X * v.Z;
        var uxvZ = u.X * v.Y - u.Y * v.X;
        return new(uxvX, uxvY, uxvZ);
    }
}
