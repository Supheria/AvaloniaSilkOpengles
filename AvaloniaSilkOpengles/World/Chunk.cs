using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;
using StbImageSharp;

namespace AvaloniaSilkOpengles.World;

public sealed class Chunk
{
    const int SizeOfChunk = 16;
    const int HeightOfChunk = 32;
    GL Gl { get; }
    Block[,,] Blocks { get; } = new Block[SizeOfChunk, HeightOfChunk, SizeOfChunk];
    List<Vector3> Vertices { get; } = [];
    List<Vector2> Uvs { get; } = [];
    List<Vector3> Normals { get; } = [];
    List<uint> Indices { get; } = [];
    uint IndexHead { get; set; }
    public Vector3 Position { get; }
    VaoHandler? Vao { get; set; }
    VboHandler<Vector3>? VertexVbo { get; set; }
    VboHandler<Vector2>? UvVbo { get; set; }
    VboHandler<Vector3>? NormalVbo { get; set; }
    IboHandler? Ibo { get; set; }
    Texture2DHandler? Texture { get; set; }
    Texture2DHandler? TextureSpecular { get; set; }

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
        Uvs.AddRange(face.Uvs);
        Normals.AddRange(face.Normals);
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
        NormalVbo = new(Gl, Normals);
        Ibo = new(Gl, Indices);

        Vao.Link(VertexVbo, 0, 3);
        Vao.Link(UvVbo, 1, 2);
        Vao.Link(NormalVbo, 2, 3);

        // Texture = new(Gl, "atlas", 1);
        Texture = new(Gl, "planks", 0, PixelFormat.Rgba);
        TextureSpecular = new(Gl, "planksSpec", 1, PixelFormat.Rgba);
    }

    public unsafe void Render(ShaderHandler shader)
    {
        shader.SetTexture(Texture);
        shader.SetTexture(TextureSpecular);
        Texture?.Bind();
        TextureSpecular?.Bind();

        Vao?.Bind();
        Ibo?.Bind();

        Gl.DrawElements(
            PrimitiveType.Triangles,
            (uint)Indices.Count,
            DrawElementsType.UnsignedInt,
            null
        );
        
        // shader.Unbind();
        Vao?.Unbind();
        Ibo?.Unbind();
        Texture?.Unbind();
        TextureSpecular?.Unbind();
    }

    public void Delete()
    {
        Vao?.Delete();
        VertexVbo?.Delete();
        UvVbo?.Delete();
        NormalVbo?.Delete();
        Ibo?.Delete();
        Texture?.Delete();
    }
}
