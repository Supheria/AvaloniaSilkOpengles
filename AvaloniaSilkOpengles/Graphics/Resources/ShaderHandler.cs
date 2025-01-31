using System;
using System.Collections.Generic;
using Avalonia.OpenGL;
using AvaloniaSilkOpengles.Assets;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class ShaderHandler : Resource
{
    private ShaderHandler(uint handle)
        : base(handle) { }

    public ShaderHandler()
        : this(0) { }

    public static ShaderHandler Create(GL gl, string shaderName, Dictionary<uint, string> attributes)
    {
        var vertexCode = AssetsRead.ReadVertex(shaderName);
        var fragmentCode = AssetsRead.ReadFragment(shaderName);
        var handle = GetHandle(gl, vertexCode, fragmentCode, attributes);
        return new(handle);
    }

    private static uint GetHandle(
        GL gl,
        string vertexCode,
        string fragmentCode,
        Dictionary<uint, string> attributes
    )
    {
        var vs = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vs, vertexCode);
        gl.CompileShader(vs);
        var error = gl.GetShaderInfoLog(vs);
        if (!string.IsNullOrEmpty(error))
            throw new ArgumentException($"Error compiling vertex shader: {error}");

        var fs = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fs, fragmentCode);
        gl.CompileShader(fs);
        error = gl.GetShaderInfoLog(fs);
        if (!string.IsNullOrEmpty(error))
            throw new ArgumentException($"Error compiling fragment shader: {error}");

        var handle = gl.CreateProgram();
        gl.AttachShader(handle, vs);
        gl.AttachShader(handle, fs);

        // foreach (var (index, name) in attributes)
        //     gl.BindAttribLocation(handle, index, name);

        gl.LinkProgram(handle);
        error = gl.GetProgramInfoLog(handle);
        if (!string.IsNullOrEmpty(error))
            throw new ArgumentException($"Error linking program: {error}");

        gl.DeleteShader(vs);
        gl.DeleteShader(fs);

        return handle;
    }

    public void Use(GL gl)
    {
        gl.UseProgram(Handle);
    }

    // public void Unbind()
    // {
    //     Gl.UseProgram(0);
    // }

    public void Delete(GL gl)
    {
        gl.DeleteProgram(Handle);
    }

    public void UniformMatrix4(GL gl, string uniformName, Matrix4 matrix)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        // csharpier-ignore
        var values = new[]{
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        };
        fixed (float* ptr = values)
        {
            gl.UniformMatrix4(location, 1, false, ptr);
        }
    }

    public void UniformCamera(GL gl, Camera camera)
    {
        UniformMatrix4(gl, "p", camera.Project);
        UniformMatrix4(gl, "v", camera.View);
    }

    public void UniformTexture(GL gl, string uniformName, Texture2D texture)
    {
        gl.ActiveTexture(TextureUnit.Texture0 + texture.Plot);
        texture.Bind(gl);
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, texture.Plot);
    }

    public void UniformFloat(GL gl, string uniformName, float value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, value);
    }

    public void UniformInt(GL gl, string uniformName, int value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, value);
    }

    public void UniformUnsignedInt(GL gl, string uniformName, uint value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, value);
    }

    public void UniformVector2(GL gl, string uniformName, Vector2 value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform2(location, value.X, value.Y);
    }

    public void UniformVector3(GL gl, string uniformName, Vector3 value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void UniformVector4(GL gl, string uniformName, Vector4 value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }
}
