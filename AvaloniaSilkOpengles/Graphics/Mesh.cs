using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public class Mesh
{
    VaoHandler? Vao { get; set; }
    VboHandler? Vbo { get; set; }
    IboHandler? Ibo { get; set; }

    public void Create(
        GL gl,
        List<Vertex> vertices,
        List<uint> indices
    )
    {
        Vao = VaoHandler.Create(gl);
        Vbo = VboHandler.Create(gl, false);
        Ibo = IboHandler.Create(gl, false);
        
        Vbo.Buffer(gl, vertices);
        Ibo.Buffer(gl, indices);

        Vao.Link(gl, Vbo, 0, 3, VertexAttribPointerType.Float, 0);
        Vao.Link(gl, Vbo, 1, 3, VertexAttribPointerType.Float, sizeof(float) * 3);
        Vao.Link(gl, Vbo, 2, 3, VertexAttribPointerType.Float, sizeof(float) * 6);
        Vao.Link(gl, Vbo, 3, 2, VertexAttribPointerType.Float, sizeof(float) * 9);

        Vao.Unbind(gl);
        Vbo.Unbind(gl);
        Ibo.Unbind(gl);
    }

    public void Delete(GL gl)
    {
        Vao?.Delete(gl);
        Vbo?.Delete(gl);
        Ibo?.Delete(gl);
    }

    public void Render(GL gl,
        PrimitiveType renderMode,
        Vector3 scale,
        Quaternion rotation,
        Vector3 translation,
        Matrix4 matrix,
        List<Texture2D> textures,
        ShaderHandler shader,
        PerspectiveCamera camera)
    {
        if (Vao is null || Vbo is null || Ibo is null)
            throw new ArgumentException("Mesh is not created yet");
        
        shader.Use(gl);
        
        Vao.Bind(gl);
        Ibo.Bind(gl);

        var diffuseCount = 0;
        var specularCount = 0;
        foreach (var texture in textures)
        {
            switch (texture.Type)
            {
                case TextureType.Diffuse:
                    shader.UniformTexture(gl, $"diffuse{diffuseCount++}", texture);
                    break;
                case TextureType.Specular:
                    shader.UniformTexture(gl, $"specular{specularCount++}", texture);
                    break;
                default:
                    continue;
            }
            texture.Bind(gl);
        }
        shader.UniformVector3(gl, "camPos", camera.Position);
        shader.UniformCamera(gl, camera);

        var scaleMatrix = Matrix4.CreateScale(scale);
        var rotationMatrix = Matrix4.CreateFromQuaternion(rotation);
        var translationMatrix = Matrix4.CreateTranslation(translation);

        shader.UniformMatrix4(gl, "scale", scaleMatrix);
        shader.UniformMatrix4(gl, "rotation", rotationMatrix);
        shader.UniformMatrix4(gl, "translation", translationMatrix);
        shader.UniformMatrix4(gl, "model", matrix);
        
        Ibo.DrawElements(gl, false);

        // Vao.Unbind();
        // Ebo.Unbind();
        // foreach (var texture in Textures)
        //     texture.Unbind();
    }
}
