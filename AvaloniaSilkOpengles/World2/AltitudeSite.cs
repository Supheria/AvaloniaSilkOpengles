using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.World2;

public class AltitudeSite
{
    public Dictionary<VertexIndex, (float X, float Z)> VertexXzs { get; }
    public TerrainType TerrainType { get; }

    public AltitudeSite(PixelPoint site, TerrainType terrainType)
    {
        VertexXzs = new()
        {
            [VertexIndex.V0] = GetXz(site, VertexIndex.V0),
            [VertexIndex.V1] = GetXz(site, VertexIndex.V1),
            [VertexIndex.V2] = GetXz(site, VertexIndex.V2),
            [VertexIndex.V3] = GetXz(site, VertexIndex.V3),
            [VertexIndex.V4] = GetXz(site, VertexIndex.V4),
        };
        TerrainType = terrainType;
    }

    private static (float X, float Z) GetXz(PixelPoint site, VertexIndex index)
    {
        (float X, float Z) xz = index switch
        {
            VertexIndex.V0 => (0.0f, 0.0f),
            VertexIndex.V1 => (-0.5f, -0.5f),
            VertexIndex.V2 => (-0.5f, 0.5f),
            VertexIndex.V3 => (0.5f, 0.5f),
            VertexIndex.V4 => (0.5f, -0.5f),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };
        return new(xz.X + site.X, xz.Z + site.Y);
    }
}
