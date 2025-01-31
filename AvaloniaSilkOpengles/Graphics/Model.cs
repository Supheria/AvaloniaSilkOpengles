using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using AvaloniaSilkOpengles.Assets.Models;
using AvaloniaSilkOpengles.Graphics.Resources;
using Microsoft.Xna.Framework;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using StbImageSharp;

namespace AvaloniaSilkOpengles.Graphics;

public class Model
{
    ModelRead ModelRead { get; }
    JsonNode JSON { get; }
    byte[] Data { get; }
    Dictionary<string, Texture2D> LoadedTextures { get; } = [];
    List<Mesh> Meshes { get; } = [];
    List<Vector3> Translations { get; } = [];
    List<Quaternion> Rotations { get; } = [];
    List<Vector3> Scales { get; } = [];
    List<Matrix4> Matrices { get; } = [];
    List<List<Texture2D>> TextureGroups { get; } = [];

    public Model(GL gl, ModelRead modelRead)
    {
        ModelRead = modelRead;
        var json = modelRead.ReadFile("scene.gltf");
        var array = JsonSerializer.Deserialize<JsonNode>(json);
        JSON = array ?? new JsonObject();

        Data = GetData();

        // LoadMesh(0);
        TraverseNode(gl, 0, Matrix4.Identity);
    }

    public void Render(GL gl, ShaderHandler? shader, PerspectiveCamera? camera)
    {
        if (shader is null || camera is null)
            return;
        for (var i = 0; i < Meshes.Count; i++)
        {
            var mesh = Meshes[i];
            mesh.Render(
                gl,
                PrimitiveType.Triangles,
                Scales[i],
                Rotations[i],
                Translations[i],
                Matrices[i],
                TextureGroups[i],
                shader,
                camera
            );
        }
    }

    public void Delete(GL gl)
    {
        foreach (var mesh in Meshes)
            mesh.Delete(gl);
    }

    private byte[] GetData()
    {
        var uri = (JSON?["buffers"]?[0]?["uri"] ?? string.Empty).ToString();
        using var stream = ModelRead.ReadFile(uri);
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    private void TraverseNode(GL gl, int nextNode, Matrix4 matrix)
    {
        var node = JSON["nodes"]?[nextNode];
        var translation = Vector3.Zero;
        var transNode = node?["translation"];
        if (transNode is not null)
        {
            translation.X = (float)(node?["translation"]?[0] ?? 0f);
            translation.Y = (float)(node?["translation"]?[1] ?? 0f);
            translation.Z = (float)(node?["translation"]?[2] ?? 0f);
        }
        var rotation = Quaternion.Identity;
        var rotationNode = node?["rotation"];
        if (rotationNode is not null)
        {
            rotation.X = (float)(node?["rotation"]?[0] ?? 0f);
            rotation.Y = (float)(node?["rotation"]?[1] ?? 0f);
            rotation.Z = (float)(node?["rotation"]?[2] ?? 0f);
            rotation.W = (float)(node?["rotation"]?[3] ?? 0f);
        }
        var scale = Vector3.One;
        var scaleNode = node?["scale"];
        if (scaleNode is not null)
        {
            scale.X = (float)(node?["scale"]?[0] ?? 0f);
            scale.Y = (float)(node?["scale"]?[1] ?? 0f);
            scale.Z = (float)(node?["scale"]?[2] ?? 0f);
        }
        var mat = Matrix4.Identity;
        var matNode = node?["matrix"];
        if (matNode is not null)
        {
            mat.M11 = (float)(node?["matrix"]?[0] ?? 0f);
            mat.M12 = (float)(node?["matrix"]?[1] ?? 0f);
            mat.M13 = (float)(node?["matrix"]?[2] ?? 0f);
            mat.M14 = (float)(node?["matrix"]?[3] ?? 0f);
            mat.M21 = (float)(node?["matrix"]?[4] ?? 0f);
            mat.M22 = (float)(node?["matrix"]?[5] ?? 0f);
            mat.M23 = (float)(node?["matrix"]?[6] ?? 0f);
            mat.M24 = (float)(node?["matrix"]?[7] ?? 0f);
            mat.M31 = (float)(node?["matrix"]?[8] ?? 0f);
            mat.M32 = (float)(node?["matrix"]?[9] ?? 0f);
            mat.M33 = (float)(node?["matrix"]?[10] ?? 0f);
            mat.M34 = (float)(node?["matrix"]?[11] ?? 0f);
            mat.M41 = (float)(node?["matrix"]?[12] ?? 0f);
            mat.M42 = (float)(node?["matrix"]?[13] ?? 0f);
            mat.M43 = (float)(node?["matrix"]?[14] ?? 0f);
            mat.M44 = (float)(node?["matrix"]?[15] ?? 0f);
        }
        var translationMatrix = Matrix4.CreateTranslation(translation);
        var rotationMatrix = Matrix4.CreateFromQuaternion(rotation);
        var scaleMatrix = Matrix4.CreateScale(scale);
        // var childMatrix = matrix * mat * transMatrix * rotationMatrix * scaleMatrix;
        var childMatrix = scaleMatrix * rotationMatrix * translationMatrix * mat * matrix;
        var meshNode = node?["mesh"];
        if (meshNode is not null)
        {
            Translations.Add(translation);
            Rotations.Add(rotation);
            Scales.Add(scale);
            Matrices.Add(matrix);

            var textures = GetTextures(gl);
            TextureGroups.Add(textures);

            var meshIndex = (int)(node?["mesh"] ?? 0);
            LoadMesh(gl, meshIndex);
        }
        var childrenNode = node?["children"];
        if (childrenNode is not null)
        {
            var count = node?["children"]?.AsArray().Count ?? 0;
            for (var i = 0; i < count; i++)
            {
                nextNode = (int)(node?["children"]?[i] ?? 0);
                TraverseNode(gl, nextNode, childMatrix);
            }
        }
    }

    private void LoadMesh(GL gl, int index)
    {
        var coordsIndex = (int)(
            JSON["meshes"]?[index]?["primitives"]?[0]?["attributes"]?["POSITION"] ?? 0
        );
        var normalsIndex = (int)(
            JSON["meshes"]?[index]?["primitives"]?[0]?["attributes"]?["NORMAL"] ?? 0
        );
        var uvsIndex = (int)(
            JSON["meshes"]?[index]?["primitives"]?[0]?["attributes"]?["TEXCOORD_0"] ?? 0
        );

        var coordsRaw = GetFloats(JSON["accessors"]?[coordsIndex]);
        var positions = GroupToVector3(coordsRaw);
        var normalRaw = GetFloats(JSON["accessors"]?[normalsIndex]);
        var normals = GroupToVector3(normalRaw);
        var uvsRaw = GetFloats(JSON["accessors"]?[uvsIndex]);
        var uvs = GroupToVector2(uvsRaw);

        var vertices = GetVertices(positions, normals, uvs);
        var indicesIndex = (int)(JSON["meshes"]?[index]?["primitives"]?[0]?["indices"] ?? 0);
        var indices = GetIndices(JSON["accessors"]?[indicesIndex]);

        var mesh = new Mesh();
        mesh.Create(gl, vertices, indices);
        Meshes.Add(mesh);
    }

    private List<Vertex> GetVertices(
        List<Vector3> positions,
        List<Vector3> normals,
        List<Vector2> uvs
    )
    {
        var vertices = new List<Vertex>();
        for (var i = 0; i < positions.Count; i++)
        {
            var vertex = new Vertex
            {
                Position = new(positions[i]),
                Normal = new(normals[i]),
                Uv = new(uvs[i]),
            };
            vertices.Add(vertex);
        }
        return vertices;
    }

    private List<float> GetFloats(JsonNode? accessor)
    {
        var floats = new List<float>();

        var bufferViewIndex = (int)(accessor?["bufferView"] ?? 0);
        var count = (int)(accessor?["count"] ?? 0);
        var byteOffsetHead = (int)(accessor?["byteOffset"] ?? 0);
        var type = (accessor?["type"] ?? string.Empty).ToString();

        var bufferView = JSON?["bufferViews"]?[bufferViewIndex];
        var byteOffset = (int)(bufferView?["byteOffset"] ?? 0);

        var elementCount = type switch
        {
            "SCALAR" => 1,
            "VEC2" => 2,
            "VEC3" => 3,
            "VEC4" => 4,
            _ => throw new ArgumentOutOfRangeException(),
        };

        var floatSize = sizeof(float);
        var dataBegin = byteOffsetHead + byteOffset;
        var dataLength = count * floatSize * elementCount;
        for (var i = dataBegin; i < dataBegin + dataLength; i += floatSize)
        {
            var bytes = new List<byte>(floatSize);
            for (var j = 0; j < floatSize; j++)
                bytes.Add(Data[i + j]);
            var value = BitConverter.ToSingle(bytes.ToArray(), 0);
            floats.Add(value);
        }

        return floats;
    }

    private List<uint> GetIndices(JsonNode? accessor)
    {
        var indices = new List<uint>();

        var bufferViewIndex = (int)(accessor?["bufferView"] ?? 0);
        var count = (int)(accessor?["count"] ?? 0);
        var byteOffsetHead = (int)(accessor?["byteOffset"] ?? 0);
        var componentType = (int)(accessor?["componentType"] ?? 0);

        var bufferView = JSON?["bufferViews"]?[bufferViewIndex];
        var byteOffset = (int)(bufferView?["byteOffset"] ?? 0);

        var dataBegin = byteOffsetHead + byteOffset;
        int valueSize,
            dataLength;

        switch (componentType)
        {
            case 5125:
                valueSize = sizeof(uint);
                dataLength = count * valueSize;
                for (var i = dataBegin; i < dataBegin + dataLength; i += valueSize)
                {
                    var bytes = new List<byte>(valueSize);
                    for (var j = 0; j < valueSize; j++)
                        bytes.Add(Data[i + j]);
                    var value = BitConverter.ToUInt32(bytes.ToArray(), 0);
                    indices.Add(value);
                }
                break;
            case 5123:
                valueSize = sizeof(ushort);
                dataLength = count * valueSize;
                for (var i = dataBegin; i < dataBegin + dataLength; i += valueSize)
                {
                    var bytes = new List<byte>(valueSize);
                    for (var j = 0; j < valueSize; j++)
                        bytes.Add(Data[i + j]);
                    var value = BitConverter.ToUInt16(bytes.ToArray(), 0);
                    indices.Add(value);
                }
                break;
            case 5122:
                valueSize = sizeof(short);
                dataLength = count * valueSize;
                for (var i = dataBegin; i < dataBegin + dataLength; i += valueSize)
                {
                    var bytes = new List<byte>(valueSize);
                    for (var j = 0; j < valueSize; j++)
                        bytes.Add(Data[i + j]);
                    var value = BitConverter.ToInt16(bytes.ToArray(), 0);
                    indices.Add((uint)value);
                }
                break;
        }

        return indices;
    }

    private List<Texture2D> GetTextures(GL gl)
    {
        var textures = new List<Texture2D>();

        var count = JSON["images"]?.AsArray().Count ?? 0;
        for (var i = 0; i < count; i++)
        {
            var name = (JSON["images"]?[i]?["uri"] ?? string.Empty).ToString();
            if (LoadedTextures.TryGetValue(name, out var loaded))
            {
                textures.Add(loaded);
                continue;
            }
            using var source = ModelRead.ReadFile(name);
            // var texture = ImageResult.FromStream(source, ColorComponents.RedGreenBlueAlpha);
            if (name.Contains("baseColor") || name.Contains("diffuse"))
            {
                var diffuse = Texture2D.Create(
                    gl,
                    source,
                    LoadedTextures.Count,
                    TextureType.Diffuse
                );
                textures.Add(diffuse);
                LoadedTextures.Add(name, diffuse);
            }
            else if (name.Contains("metallicRoughness") || name.Contains("specular"))
            {
                var specular = Texture2D.Create(
                    gl,
                    source,
                    LoadedTextures.Count,
                    TextureType.Specular
                );
                textures.Add(specular);
                LoadedTextures.Add(name, specular);
            }
        }

        return textures;
    }

    private List<Vector2> GroupToVector2(List<float> floats)
    {
        var vectors = new List<Vector2>();
        for (var i = 0; i < floats.Count; i += 2)
        {
            var vector = new Vector2
            {
                X = floats[i],
                Y = floats[i + 1]
            };
            vectors.Add(vector);
        }
        return vectors;
    }

    private List<Vector3> GroupToVector3(List<float> floats)
    {
        var vectors = new List<Vector3>();
        for (var i = 0; i < floats.Count; i += 3)
        {
            var vector = new Vector3
            {
                X = floats[i],
                Y = floats[i + 1],
                Z = floats[i + 2]
            };
            vectors.Add(vector);
        }
        return vectors;
    }
}
