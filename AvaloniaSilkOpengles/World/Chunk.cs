using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Assets.Textures;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;
using StbImageSharp;

namespace AvaloniaSilkOpengles.World;

public sealed class Chunk : RenderableObject
{
    const int SizeOfChunk = 16;
    const int HeightOfChunk = 32;
    Block[,,] Blocks { get; } = new Block[SizeOfChunk, HeightOfChunk, SizeOfChunk];
    uint IndexHead { get; set; }

    public Chunk(GL gl)
        : base(gl)
    {
        var heightMap = GenerateHeightMap();
        GenerateBlocks(heightMap);
        GenerateBlockFaces();
        CreateMesh();
        AddTextures(gl);
    }

    private float[,] GenerateHeightMap()
    {
        var heightMap = new float[SizeOfChunk, SizeOfChunk];
        SimplexNoise.Noise.Seed = 123456;
        for (var x = 0; x < SizeOfChunk; x++)
        {
            for (var z = 0; z < SizeOfChunk; z++)
            {
                heightMap[x, z] = SimplexNoise.Noise.CalcPixel2D(x, z, 0.01f);
            }
        }
        return heightMap;
    }

    private void GenerateBlocks(float[,] heightMap)
    {
        for (var x = 0; x < SizeOfChunk; x++)
        {
            for (var z = 0; z < SizeOfChunk; z++)
            {
                var columnHeight = (int)(heightMap[x, z] / 10);
                for (var y = 0; y < HeightOfChunk; y++)
                {
                    var type = BlockType.Empty;
                    // if (y < columnHeight - 1)
                    //     type = BlockType.Dirt;
                    // else if (y == columnHeight - 1)
                    //     type = BlockType.Grass;
                    if (y <= columnHeight - 1)
                        type = BlockType.TestBlock;
                    var offset = new Vector3(x, y, z);
                    Blocks[x, y, z] = new Block(offset, type);
                }
            }
        }
        // Blocks[0, 0, 0] = new(Vector3.Zero, BlockType.Glass);
    }

    private void GenerateBlockFaces()
    {
        for (var x = 0; x < SizeOfChunk; x++)
        {
            for (var z = 0; z < SizeOfChunk; z++)
            {
                // var columnHeight = (int)(heightMap[x, z] / 10);
                for (var y = 0; y < HeightOfChunk; y++)
                {
                    var block = Blocks[x, y, z];
                    if (block.Type is BlockType.Empty)
                        continue;
                    // right faces
                    if (x < SizeOfChunk - 1)
                    {
                        if (Blocks[x + 1, y, z].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Right]);
                    }
                    else
                        AddBlockFace(block[FaceType.Right]);
                    // left faces
                    if (x > 0)
                    {
                        if (Blocks[x - 1, y, z].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Left]);
                    }
                    else
                        AddBlockFace(block[FaceType.Left]);
                    // top faces
                    if (y < HeightOfChunk - 1)
                    {
                        if (Blocks[x, y + 1, z].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Top]);
                    }
                    else
                        AddBlockFace(block[FaceType.Top]);
                    // bottom face
                    if (y > 0)
                    {
                        if (Blocks[x, y - 1, z].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Bottom]);
                    }
                    else
                        AddBlockFace(block[FaceType.Bottom]);
                    // front face
                    if (z < SizeOfChunk - 1)
                    {
                        if (Blocks[x, y, z + 1].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Front]);
                    }
                    else
                        AddBlockFace(block[FaceType.Front]);
                    // back face
                    if (z > 0)
                    {
                        if (Blocks[x, y, z - 1].Type is BlockType.Empty)
                            AddBlockFace(block[FaceType.Back]);
                    }
                    else
                        AddBlockFace(block[FaceType.Back]);
                }
            }
        }
        // var block = Blocks[0, 0, 0];
        // AddBlockFace(block[Face.Front]);
        // AddBlockFace(block[Face.Right]);
        // AddBlockFace(block[Face.Back]);
        // AddBlockFace(block[Face.Left]);
        // AddBlockFace(block[Face.Top]);
        // AddBlockFace(block[Face.Bottom]);
    }

    private void AddBlockFace(BlockFace face)
    {
        AddVertices(face);
        AddTriangleIndices(0 + IndexHead, 1 + IndexHead, 2 + IndexHead);
        AddTriangleIndices(2 + IndexHead, 3 + IndexHead, 0 + IndexHead);
        IndexHead += 4;
    }

    private void AddTextures(GL gl)
    {
        using var source1 = TextureRead.Read("planks");
        using var source2 = TextureRead.Read("planksSpec");
        // var texture = ImageResult.FromStream(source1, ColorComponents.RedGreenBlueAlpha);
        var diffuse = new Texture2DHandler(gl, source1, TextureType.Diffuse, 0);
        // texture = ImageResult.FromStream(source2, ColorComponents.RedGreenBlueAlpha);
        var specular = new Texture2DHandler(gl, source2, TextureType.Specular, 1);
        Textures.AddRange([diffuse, specular]);
    }
}
