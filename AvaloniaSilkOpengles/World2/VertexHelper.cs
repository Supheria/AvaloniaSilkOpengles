using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Avalonia;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.World2;

public sealed class VertexHelper
{
    public static Vector3[] GetCoords(
        FaceType faceType,
        float altitude1,
        float altitude2,
        float altitude3
    )
    {
        return faceType switch
        {
            FaceType.Back =>
            [
                GetCoord(1, altitude1),
                GetCoord(0, altitude2),
                GetCoord(4, altitude3),
            ],
            FaceType.Left =>
            [
                GetCoord(2, altitude1),
                GetCoord(0, altitude2),
                GetCoord(1, altitude3),
            ],
            FaceType.Front =>
            [
                GetCoord(3, altitude1),
                GetCoord(0, altitude2),
                GetCoord(2, altitude3),
            ],
            FaceType.Right =>
            [
                GetCoord(4, altitude1),
                GetCoord(0, altitude2),
                GetCoord(3, altitude3),
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(faceType), faceType, null)
        };
    }
    
    private static Vector3 GetCoord(int index, float altitude)
    {
        return index switch
        {
            0 => new(0.0f, altitude, 0.0f), // center
            1 => new(-0.5f, altitude, -0.5f), // back left
            2 => new(-0.5f, altitude, 0.5f), // front left
            3 => new(0.5f, altitude, 0.5f), // front right
            4 => new(0.5f, altitude, -0.5f), // back right
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
    }
    
    public static Vector2 GetXz(VertexIndex index)
    {
        return index switch
        {
            VertexIndex.V0 => new(0.0f, 0.0f),
            VertexIndex.V1 => new(-0.5f, -0.5f),
            VertexIndex.V2 => new(-0.5f, 0.5f),
            VertexIndex.V3 => new(0.5f, 0.5f),
            VertexIndex.V4 => new(0.5f, -0.5f),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
            
    }
    
    public static List<uint[]> GetIndices(uint start)
    {
        return
        [
            GroupIndices(start, 1, 0, 4),
            GroupIndices(start, 2, 0, 1),
            GroupIndices(start, 3, 0, 2),
            GroupIndices(start, 4, 0, 3),
        ];
    }

    private static uint[] GroupIndices(uint start, uint e1, uint e2, uint e3)
    {
        return [e1 + start, e2 + start, e3 + start];
    }

    public static Vector3 GetColor(TerrainType type)
    {
        return type switch
        {
            TerrainType.None => NormalizeColor(Color.Black),
            TerrainType.Plain => NormalizeColor(Color.LightYellow),
            TerrainType.Stream => NormalizeColor(Color.SkyBlue),
            TerrainType.Wood => NormalizeColor(Color.LimeGreen),
            TerrainType.Hill => NormalizeColor(Color.DarkSlateGray),
        };
    }

    private static Vector3 NormalizeColor(Color color)
    {
        return new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
    }
}
