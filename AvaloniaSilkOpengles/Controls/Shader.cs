using System;
using System.Numerics;
using Avalonia.OpenGL;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Controls;

public unsafe class Shader
{
    string VertexCode { get; }
    string FragmentCode { get; }
    int ProgramHandler { get; set; }

    public Shader(string vertexCode, string fragmentCode)
    {
        VertexCode = vertexCode;
        FragmentCode = fragmentCode;
    }

    public void Load(GlInterface gl)
    {
        var vs = gl.CreateShader(GL_VERTEX_SHADER);
        var error = gl.CompileShaderAndGetError(vs, VertexCode);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error compiling vertex shader: {error}");

        var fs = gl.CreateShader(GL_FRAGMENT_SHADER);
        error = gl.CompileShaderAndGetError(fs, FragmentCode);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error compiling fragment shader: {error}");

        ProgramHandler = gl.CreateProgram();
        gl.AttachShader(ProgramHandler, vs);
        gl.AttachShader(ProgramHandler, fs);

        error = gl.LinkProgramAndGetError(ProgramHandler);
        if (!string.IsNullOrEmpty(error))
            throw new Exception($"Error linking program: {error}");

        gl.DeleteShader(vs);
        gl.DeleteShader(fs);
    }

    public void Use(GlInterface gl)
    {
        gl.UseProgram(ProgramHandler);
    }

    public void Delete(GlInterface gl)
    {
        gl.DeleteProgram(ProgramHandler);
    }

    public void SetMatrix(GlInterface gl, string uniformName, Matrix4x4 matrix)
    {
        var location = gl.GetUniformLocationString(ProgramHandler, uniformName);
        var values = GetMatrix4X4Values(matrix);
        fixed (float* v = &values[0])
        {
            gl.UniformMatrix4fv(location, 1, false, v);
        }
    }

    // csharpier-ignore
    private float[] GetMatrix4X4Values(Matrix4x4 matrix)
    {
        return [
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        ];
    }
    // private float[] GetMatrix4X4Values(Matrix4x4 matrix)
    // {
    //     return [
    //         matrix.M11,matrix.M21, matrix.M31, matrix.M41,
    //         matrix.M12, matrix.M22, matrix.M23, matrix.M42,
    //         matrix.M13, matrix.M23, matrix.M33, matrix.M43,
    //         matrix.M14, matrix.M24, matrix.M34, matrix.M44,
    //     ];
    // }
}
