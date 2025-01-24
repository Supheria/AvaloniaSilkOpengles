using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;

namespace AvaloniaSilkOpengles.World;

public sealed class VertexData
{
    public static Dictionary<FaceType, Position[]> Positions { get; } =
        new()
        {
            [FaceType.Front] =
            [
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
            ],
            [FaceType.Right] =
            [
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
            ],
            [FaceType.Back] =
            [
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
            ],
            [FaceType.Left] =
            [
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, 0.5f, 0.5f),
            ],
            [FaceType.Top] =
            [
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
            ],
            [FaceType.Bottom] =
            [
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
            ],
        };
    public static Dictionary<FaceType, Normal[]> Normals { get; } =
        new()
        {
            [FaceType.Front] =
            [
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
            ],
            [FaceType.Right] =
            [
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
            ],
            [FaceType.Back] =
            [
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
            ],
            [FaceType.Left] =
            [
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
            ],
            [FaceType.Top] =
            [
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
            ],
            [FaceType.Bottom] =
            [
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
            ],
        };
}
