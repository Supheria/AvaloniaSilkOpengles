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

    public void Create(
        GL gl,
        List<Vertex> vertices,
        List<uint> indices
    )
    {
        Vao = new(gl);
        Vbo = new(gl, vertices);
        Ebo = new(gl, indices);

        Vao.Link(gl, Vbo);

        Vao.Unbind(gl);
        Vbo.Unbind(gl);
        Ebo.Unbind(gl);
    }

    public void Delete(GL gl)
    {
        Vao?.Delete(gl);
        Vbo?.Delete(gl);
        Ebo?.Delete(gl);
    }

    public void Render(GL gl,
        PrimitiveType renderMode,
        Vector3 scale,
        Quaternion rotation,
        Vector3 translation,
        Matrix4x4 matrix,
        List<Texture2DHandler> textures,
        ShaderHandler shader,
        Camera3D camera)
    {
        if (Vao is null || Vbo is null || Ebo is null)
            throw new ArgumentException("Mesh is not created yet");
        
        Vao.Bind(gl);
        Ebo.Bind(gl);

        var diffuseCount = 0;
        var specularCount = 0;
        foreach (var texture in textures)
        {
            switch (texture.Type)
            {
                case TextureType.Diffuse:
                    shader.SetTexture(gl, texture, $"diffuse{diffuseCount++}");
                    break;
                case TextureType.Specular:
                    shader.SetTexture(gl, texture, $"specular{specularCount++}");
                    break;
                default:
                    continue;
            }
            texture.Bind(gl);
        }
        shader.SetVector3(gl, "camPos", camera.Position);
        shader.SetMatrix(gl, "camMatrix", camera.GetMatrix());

        var scaleMatrix = Matrix4x4.CreateScale(scale);
        var rotationMatrix = Matrix4x4.CreateFromQuaternion(rotation);
        var translationMatrix = Matrix4x4.CreateTranslation(translation);

        shader.SetMatrix(gl, "scale", scaleMatrix);
        shader.SetMatrix(gl, "rotation", rotationMatrix);
        shader.SetMatrix(gl, "translation", translationMatrix);
        shader.SetMatrix(gl, "model", matrix);
        
        Ebo.DrawElements(gl, renderMode);

        // Vao.Unbind();
        // Ebo.Unbind();
        // foreach (var texture in Textures)
        //     texture.Unbind();
    }
}
