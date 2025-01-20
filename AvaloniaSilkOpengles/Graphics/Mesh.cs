using System;
using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public unsafe class Mesh
{
    GL Gl { get; }
    List<Vertex> Vertices { get; }
    List<uint> Indices { get; }
    List<Texture2DHandler> Textures { get; }
    VaoHandler Vao { get; }
    VboHandler Vbo { get; }
    EboHandler Ebo { get; }

    public Mesh(GL gl, List<Vertex> vertices, List<uint> indices, List<Texture2DHandler> textures)
    {
        Gl = gl;
        Vertices = vertices;
        Indices = indices;
        Textures = textures;

        Vao = new(gl);
        Vbo = new(gl, vertices);
        Ebo = new(gl, indices);

        Vao.Link(Vbo);

        Vao.Unbind();
        Vbo.Unbind();
        Ebo.Unbind();
    }

    public void Delete()
    {
        Vao.Delete();
        Vbo.Delete();
        Ebo.Delete();
        foreach (var texture in Textures)
            texture.Delete();
    }

    public void Render(ShaderHandler shader, Camera3D camera, Vector3 scale, Quaternion rotation, Vector3 translation, Matrix4x4 matrix)
    {
        Vao.Bind();
        Ebo.Bind();

        var diffuseCount = 0;
        var specularCount = 0;
        foreach (var texture in Textures)
        {
            switch (texture.Type)
            {
                case TextureType.Diffuse:
                    shader.SetTexture(texture, $"diffuse{diffuseCount++}");
                    break;
                case TextureType.Specular:
                    shader.SetTexture(texture, $"specular{specularCount++}");
                    break;
                default:
                    continue;
            }
            texture.Bind();
        }
        shader.SetVector3("camPos", camera.Position);
        shader.SetMatrix("camMatrix", camera.GetMatrix());
        
        var scaleMatrix = Matrix4x4.CreateScale(scale);
        var rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
        var translationMatrix = Matrix4x4.CreateTranslation(translation);
        
        shader.SetMatrix("scale", scaleMatrix);
        shader.SetMatrix("rotation", rotationMatrix);
        shader.SetMatrix("translation", translationMatrix);
        shader.SetMatrix("model", matrix);

        Gl.DrawElements(
            PrimitiveType.Triangles,
            (uint)Indices.Count,
            DrawElementsType.UnsignedInt,
            null
        );

        // Vao.Unbind();
        // Ebo.Unbind();
        // foreach (var texture in Textures)
        //     texture.Unbind();
    }
}
