using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class LightCube : RenderableObject
{
    uint IndexHead { get; set; }

    public LightCube(GL gl)
        : base(gl)
    {
        GenerateFaces();
        CreateMesh();
    }

    private void GenerateFaces()
    {
        AddFace(FaceType.Front);
        AddFace(FaceType.Right);
        AddFace(FaceType.Back);
        AddFace(FaceType.Left);
        AddFace(FaceType.Top);
        AddFace(FaceType.Bottom);
    }

    private void AddFace(FaceType faceType)
    {
        var positions = VertexData.Positions[faceType];
        Face face;
        switch (faceType)
        {
            case FaceType.Front:
                face = new LightCubeFace(positions, new(Color.Red));
                break;
            case FaceType.Right:
                face = new LightCubeFace(positions, new(Color.Green));
                break;
            case FaceType.Top:
                face = new LightCubeFace(positions, new(Color.Blue));
                break;
            default:
                face = new LightCubeFace(positions, new(Color.White));
                break;
        }
        AddVertices(face);
        AddTriangleIndices(0 + IndexHead, 1 + IndexHead, 2 + IndexHead);
        AddTriangleIndices(2 + IndexHead, 3 + IndexHead, 0 + IndexHead);
        IndexHead += 4;
    }
}
