using System.Collections.Generic;
using System.Numerics;
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
    public Vector3 Position { get; }

    public Chunk(GL gl, Vector3 position)
    {
        Position = position;
        var heightMap = GenerateHeightMap();
        GenerateBlocks(heightMap);
        GenerateBlockFaces();
        Mesh = BuildMesh(gl, Vertices, Indices);
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
                    var position = new Vector3(x, y, z);
                    Blocks[x, y, z] = new Block(position, type);
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
                            AddBlockFace(block[Face.Right]);
                    }
                    else
                        AddBlockFace(block[Face.Right]);
                    // left faces
                    if (x > 0)
                    {
                        if (Blocks[x - 1, y, z].Type is BlockType.Empty)
                            AddBlockFace(block[Face.Left]);
                    }
                    else
                        AddBlockFace(block[Face.Left]);
                    // top faces
                    if (y < HeightOfChunk - 1)
                    {
                        if (Blocks[x, y + 1, z].Type is BlockType.Empty)
                            AddBlockFace(block[Face.Top]);
                    }
                    else
                        AddBlockFace(block[Face.Top]);
                    // bottom face
                    if (y > 0)
                    {
                        if (Blocks[x, y - 1, z].Type is BlockType.Empty)
                            AddBlockFace(block[Face.Bottom]);
                    }
                    else
                        AddBlockFace(block[Face.Bottom]);
                    // front face
                    if (z < SizeOfChunk - 1)
                    {
                        if (Blocks[x, y, z + 1].Type is BlockType.Empty)
                            AddBlockFace(block[Face.Front]);
                    }
                    else
                        AddBlockFace(block[Face.Front]);
                    // back face
                    if (z > 0)
                    {
                        if (Blocks[x, y, z - 1].Type is BlockType.Empty)
                            AddBlockFace(block[Face.Back]);
                    }
                    else
                        AddBlockFace(block[Face.Back]);
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
        Vertices.AddRange(face.Vertices);
        Indices.AddRange(
            [
                0 + IndexHead,
                1 + IndexHead,
                2 + IndexHead,
                2 + IndexHead,
                3 + IndexHead,
                0 + IndexHead,
            ]
        );
        IndexHead += 4;
    }

    private static Mesh BuildMesh(GL gl, List<Vertex> vertices, List<uint> indices)
    {
        var diffuse = new Texture2DHandler(gl, "planks", TextureType.Diffuse, 0);
        var specular = new Texture2DHandler(gl, "planksSpec", TextureType.Specular, 1);
        var textures = new List<Texture2DHandler>
        {
            diffuse,
            specular,
        };
        return new Mesh(gl, vertices, indices, textures);
    }
}
