using System;
using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public class Mesh
{
    VaoHandler? Vao { get; set; }
    VboHandler? Vbo { get; set; }
    EboHandler? Ebo { get; set; }
    List<Texture2DHandler> Textures { get; set; } = [];
    public Vector3 Scale { get; set; } = Vector3.One;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3 Translation { get; set; } = Vector3.Zero;
    public Matrix4x4 Matrix { get; set; } = Matrix4x4.Identity;
    public PrimitiveType RenderMode { get; set; } = PrimitiveType.Triangles;

    public void Create(
        GL gl,
        List<Vertex> vertices,
        List<uint> indices,
        List<Texture2DHandler> textures
    )
    {
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
        Vao?.Delete();
        Vbo?.Delete();
        Ebo?.Delete();
        foreach (var texture in Textures)
            texture.Delete();
    }

    public void Render(ShaderHandler shader, Camera3D camera)
    {
        if (Vao is null || Vbo is null || Ebo is null)
            throw new ArgumentException("Mesh is not created yet");
        
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

        var scaleMatrix = Matrix4x4.CreateScale(Scale);
        var rotationMatrix = Matrix4x4.CreateFromQuaternion(Rotation);
        var translationMatrix = Matrix4x4.CreateTranslation(Translation);

        shader.SetMatrix("scale", scaleMatrix);
        shader.SetMatrix("rotation", rotationMatrix);
        shader.SetMatrix("translation", translationMatrix);
        shader.SetMatrix("model", Matrix);
        
        Ebo.DrawElements(RenderMode);

        // Vao.Unbind();
        // Ebo.Unbind();
        // foreach (var texture in Textures)
        //     texture.Unbind();
    }
}
