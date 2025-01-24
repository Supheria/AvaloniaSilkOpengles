using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using AvaloniaSilkOpengles.Assets.Textures;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Sphere;

public class IcoSphere : RenderableObject
{
    List<Position> Positions { get; } = [];
    uint IndexHead { get; set; }
    Texture2DHandler Texture { get; set; }

    public IcoSphere(GL gl, int recursionLevel = 3)
        : base(gl)
    {
        GeneratePostitions();
        GenerateFaces(recursionLevel);
        using var stream = TextureRead.Read("wood");
        Texture = new(gl, stream, TextureType.Diffuse, 0);
        Textures.Add(Texture);
        CreateMesh();
    }

    private void GeneratePostitions()
    {
        var t = (1 + MathF.Sqrt(5)) / 2;
        Positions.AddRange([
            new Position(-1, t, 0).Normalize(),
            new Position(1, t, 0).Normalize(),
            new Position(-1, -t, 0).Normalize(),
            new Position(1, -t, 0).Normalize(),

            new Position(0, -1, t).Normalize(),
            new Position(0, 1, t).Normalize(),
            new Position(0, -1, -t).Normalize(),
            new Position(0, 1, -t).Normalize(),

            new Position(t, 0, -1).Normalize(),
            new Position(t, 0, 1).Normalize(),
            new Position(-t, 0, -1).Normalize(),
            new Position(-t, 0, 1).Normalize(),
        ]);
    }

    private void GenerateFaces(int recursionLevel)
    {
        var faces = new List<SphereFace>
        {
            // 5 faces around point 0
            new(Positions[0], Positions[11], Positions[5]),
            new(Positions[0], Positions[5], Positions[1]),
            new(Positions[0], Positions[1], Positions[7]),
            new(Positions[0], Positions[7], Positions[10]),
            new(Positions[0], Positions[10], Positions[11]),
            // 5 adjacent faces
            new(Positions[1], Positions[5], Positions[9]),
            new(Positions[5], Positions[11], Positions[4]),
            new(Positions[11], Positions[10], Positions[2]),
            new(Positions[10], Positions[7], Positions[6]),
            new(Positions[7], Positions[1], Positions[8]),
            // 5 faces around point 3
            new(Positions[3], Positions[9], Positions[4]),
            new(Positions[3], Positions[4], Positions[2]),
            new(Positions[3], Positions[2], Positions[6]),
            new(Positions[3], Positions[6], Positions[8]),
            new(Positions[3], Positions[8], Positions[9]),
            // 5 adjacent faces
            new(Positions[4], Positions[9], Positions[5]),
            new(Positions[2], Positions[4], Positions[11]),
            new(Positions[6], Positions[2], Positions[10]),
            new(Positions[8], Positions[6], Positions[7]),
            new(Positions[9], Positions[8], Positions[1]),
        };

        for (var i = 0; i < recursionLevel; i++)
        {
            var refinedFaces = new List<SphereFace>();
            foreach (var face in faces)
            {
                var m1 = GetMiddlePosition(face.P1, face.P2);
                var m2 = GetMiddlePosition(face.P2, face.P3);
                var m3 = GetMiddlePosition(face.P3, face.P1);

                refinedFaces.AddRange(
                    [
                        new(face.P1, m1, m3),
                        new(face.P2, m2, m1),
                        new(face.P3, m3, m2),
                        new(m1, m2, m3),
                    ]
                );
            }
            faces = refinedFaces;
        }

        foreach (var face in faces)
            AddFace(face);
    }

    private Position GetMiddlePosition(Position pos1, Position pos2)
    {
        return new Position(
            (pos1.X + pos2.X) / 2f,
            (pos1.Y + pos2.Y) / 2f,
            (pos1.Z + pos2.Z) / 2f
        ).Normalize();
    }

    private void AddFace(SphereFace face)
    {
        var uv1 = GetUv(face.P1);
        var uv2 = GetUv(face.P2);
        var uv3 = GetUv(face.P3);
        face.GenerateVertices(uv1, uv2, uv3);
        AddVertices(face);
        AddTriangleIndices(IndexHead++, IndexHead++, IndexHead++);
    }

    private static TexUv GetUv(Position pos)
    {
        var length = pos.Value.Length();
        var x = -((MathF.Atan2(pos.Z, pos.X) / MathF.PI + 1) * 0.5f);
        var y = MathF.Acos(pos.Y / length) / MathF.PI;
        return new(x, y);
    }
}
