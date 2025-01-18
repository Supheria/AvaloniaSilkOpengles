using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public sealed class FaceData
{
    public static Dictionary<Face, Vector3[]> VertexData { get; } =
        new()
        {
            [Face.Front] =
            [
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
            ],
            [Face.Right] =
            [
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
            ],
            [Face.Back] =
            [
                new(0.5f, 0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
            ],
            [Face.Left] =
            [
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, 0.5f, 0.5f),
            ],
            [Face.Top] =
            [
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
            ],
            [Face.Bottom] =
            [
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
            ],
        };
    public static Dictionary<Face, Vector3[]> NormalData { get; } =
        new()
        {
            [Face.Front] =
            [
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
                new(0.0f, 0.0f, 1.0f),
            ],
            [Face.Right] =
            [
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
                new(1.0f, 0.0f, 0.0f),
            ],
            [Face.Back] =
            [
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
                new(0.0f, 0.0f, -1.0f),
            ],
            [Face.Left] =
            [
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
                new(-1.0f, 0.0f, 0.0f),
            ],
            [Face.Top] =
            [
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
                new(0.0f, 1.0f, 0.0f),
            ],
            [Face.Bottom] =
            [
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
                new(0.0f, -1.0f, 0.0f),
            ],
        };
}
