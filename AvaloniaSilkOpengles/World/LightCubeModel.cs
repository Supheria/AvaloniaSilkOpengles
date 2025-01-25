using Avalonia.Media;
using AvaloniaSilkOpengles.Graphics;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public class LightCubeModel : RenderableObject
{
    uint IndexHead { get; set; }

    public LightCubeModel(GL gl)
    {
        GenerateFaces();
        CreateMesh(gl);
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
                face = new LightCubeFace(positions, new(Colors.Red));
                break;
            case FaceType.Right:
                face = new LightCubeFace(positions, new(Colors.Green));
                break;
            case FaceType.Top:
                face = new LightCubeFace(positions, new(Colors.Blue));
                break;
            default:
                face = new LightCubeFace(positions, new(Colors.White));
                break;
        }
        AddVertices(face);
        AddTriangleIndices(0 + IndexHead, 1 + IndexHead, 2 + IndexHead);
        AddTriangleIndices(2 + IndexHead, 3 + IndexHead, 0 + IndexHead);
        IndexHead += 4;
    }
}