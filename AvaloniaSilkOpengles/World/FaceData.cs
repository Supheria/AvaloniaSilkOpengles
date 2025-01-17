using System.Collections.Generic;
using System.Numerics;

namespace AvaloniaSilkOpengles.World;

public sealed class FaceData
{
    public static Dictionary<Face, Vector3[]> RawVertexData { get; } =
        new()
        {
            [Face.Front] =
            [
                new(-0.5f, 0.5f, 0.5f),
                new(-0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
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
            [Face.Right] =
            [
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, -0.5f, 0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, 0.5f, -0.5f),
            ],
            [Face.Top] =
            [
                new(-0.5f, 0.5f, -0.5f),
                new(-0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, 0.5f),
                new(0.5f, 0.5f, -0.5f),
            ],
            [Face.Bottom] =
            [
                new(-0.5f, -0.5f, 0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, -0.5f),
                new(0.5f, -0.5f, 0.5f),
            ],
        };
}
