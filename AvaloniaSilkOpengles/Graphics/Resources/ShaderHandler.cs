using System;
using Avalonia.OpenGL;
using AvaloniaSilkOpengles.Assets.Shaders;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class ShaderHandler : ResourceHandler
{
    public ShaderHandler(GL gl, string shaderName)
    {
        var vertexCode = ShaderRead.ReadVertex(shaderName);
        var fragmentCode = ShaderRead.ReadFragment(shaderName);
        Handle = Load(gl, vertexCode, fragmentCode);
    }

    private static uint Load(GL gl, string vertexCode, string fragmentCode)
    {
        var vs = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vs, vertexCode);
        gl.CompileShader(vs);
        var error = gl.GetShaderInfoLog(vs);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error compiling vertex shader: {error}");

        var fs = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fs, fragmentCode);
        gl.CompileShader(fs);
        error = gl.GetShaderInfoLog(fs);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error compiling fragment shader: {error}");

        var handle = gl.CreateProgram();
        gl.AttachShader(handle, vs);
        gl.AttachShader(handle, fs);

        gl.LinkProgram(handle);
        error = gl.GetProgramInfoLog(handle);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error linking program: {error}");

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

    public void SetMatrix(GL gl, string uniformName, Matrix4 matrix)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        var values = GetMatrix4X4Values(matrix);
        fixed (float* ptr = values)
        {
            gl.UniformMatrix4(location, 1, false, ptr);
        }
    }

    // csharpier-ignore
    private float[] GetMatrix4X4Values(Matrix4 matrix)
    {
        return [
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        ];
    }

    public void SetValue(GL gl, string uniformName, float value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, value);
    }

    public void SetTexture(GL gl, Texture2DHandler? texture, string uniformName)
    {
        if (texture is null)
            return;
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform1(location, texture.Plot);
    }

    public void SetVector3(GL gl, string uniformName, Vector3 value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform3(location, [value.X, value.Y, value.Z]);
    }

    public void SetVector4(GL gl, string uniformName, Vector4 value)
    {
        var location = gl.GetUniformLocation(Handle, uniformName);
        gl.Uniform4(location, [value.X, value.Y, value.Z, value.W]);
    }
}
