using System;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VaoHandler : ResourceHandler
{
    public VaoHandler(GL gl, bool doUnbind = false) : base(gl)
    {
        Handle = gl.GenVertexArray();
        if (!doUnbind)
            gl.BindVertexArray(Handle);
    }

    public void Link<T>(
        uint plot,
        int elementCount,
        VertexAttribPointerType type,
        VboHandler<T> vbo,
        bool normalized = false
    )
        where T : unmanaged
    {
        vbo.Bind();
        var stride = (uint)sizeof(T);
        Gl.VertexAttribPointer(plot, elementCount, type, normalized, stride, 0);
        Gl.EnableVertexAttribArray(plot);
    }

    public void Bind()
    {
        Gl.BindVertexArray(Handle);
    }

    public void Unbind()
    {
        Gl.BindVertexArray(0);
    }

    public void Delete()
    {
        Gl.DeleteVertexArray(Handle);
    }
}
