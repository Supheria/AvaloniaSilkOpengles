using System.Collections.Generic;
using System.Numerics;
using Avalonia;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.World;

public struct TexCoord
{
    const float ColumnOfAtlas = 16f;
    const float RowOfAtlas = 16f;
    public TexUv[] Uvs { get; }

    private TexCoord(TexUv[] uvs)
    {
        Uvs = uvs;
    }

    public static TexCoord CreateFromAtlas(int column, int row)
    {
        var left = column / ColumnOfAtlas;
        var top = row / RowOfAtlas;
        var right = (column + 1) / ColumnOfAtlas;
        var bottom = (row + 1) / RowOfAtlas;
        var uvs = new TexUv[]
        {
            new(left, top),
            new(left, bottom),
            new(right, bottom),
            new(right, top),
        };
        return new(uvs);
    }
    
    public static TexCoord CreateFromSingle()
    {
        const int left = 0;
        const int top = 0;
        const int right = 1;
        const int bottom = 1;
        var uvs = new TexUv[]
        {
            new(left, top),
            new(left, bottom),
            new(right, bottom),
            new(right, top),
        };
        return new(uvs);
    }
}
