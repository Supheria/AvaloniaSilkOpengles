using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class Chunk
{
    const int SizeOfChunk = 16;
    const int HeightOfChunk = 32;
    GL Gl { get; }

    Block[,,] Blocks { get; } = new Block[SizeOfChunk, HeightOfChunk, SizeOfChunk];
    List<Vector3> Vertices { get; } = [];
    List<Vector2> Uvs { get; } = [];
    List<uint> Indices { get; } = [];
    uint IndexHead { get; set; }
    public Vector3 Position { get; }
    VaoHandler? Vao { get; set; }
    VboHandler<Vector3>? VertexVbo { get; set; }
    VboHandler<Vector2>? UvVbo { get; set; }
    EboHandler? Ebo { get; set; }
    Texture2DHandler? Texture { get; set; }

    public Chunk(GL gl, Vector3 position)
    {
        Gl = gl;
        Position = position;
        var heightMap = GenerateHeightMap();
        GenerateBlocks(heightMap);
        GenerateBlockFaces();
        Build();
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
                    if (y < columnHeight - 1)
                        type = BlockType.Dirt;
                    else if (y == columnHeight - 1)
                        type = BlockType.Grass;
                    var position = new Vector3(x, y, z);
                    Blocks[x, y, z] = new Block(position, type);
                }
            }
        }
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
    }

    private void AddBlockFace(BlockFace face)
    {
        Vertices.AddRange(face.Vertices);
        Uvs.AddRange(face.Uvs);
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

    private void Build()
    {
        Vao = new(Gl);
        VertexVbo = new(Gl, Vertices);
        UvVbo = new(Gl, Uvs);
        Ebo = new(Gl, Indices);

        Vao.Link(0, 3, VertexVbo, VertexAttribPointerType.Float);
        Vao.Link(1, 2, UvVbo, VertexAttribPointerType.Float);

        Texture = new(Gl, "atlas", 0);
    }

    public unsafe void Render(ShaderHandler shader)
    {
        shader.Bind();
        shader.SetTexture(Texture);
        Texture?.Bind();

        Vao?.Bind();
        Ebo?.Bind();
        Gl.DrawElementsInstanced(
            PrimitiveType.Triangles,
            (uint)Indices.Count,
            DrawElementsType.UnsignedInt,
            null,
            1
        );
    }

    public void Delete()
    {
        Vao?.Delete();
        VertexVbo?.Delete();
        UvVbo?.Delete();
        Ebo?.Delete();
        Texture?.Delete();
    }
}
