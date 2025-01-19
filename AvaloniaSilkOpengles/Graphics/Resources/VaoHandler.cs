using System;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VaoHandler : ResourceHandler
{
    public VaoHandler(GL gl, bool doUnbind = false)
        : base(gl)
    {
        Handle = gl.GenVertexArray();
        if (!doUnbind)
            gl.BindVertexArray(Handle);
    }

    // public void Link<T>(
    //     VboHandler<T> vbo,
    //     uint plot,
    //     int elementCount,
    //     VertexAttribPointerType elementType = VertexAttribPointerType.Float,
    //     bool normalized = false
    // )
    //     where T : unmanaged
    // {
    //     vbo.Bind();
    //     var stride = (uint)sizeof(T);
    //     Gl.VertexAttribPointer(plot, elementCount, elementType, normalized, stride, null);
    //     Gl.EnableVertexAttribArray(plot);
    // }

    public void Link<T>(
        VboHandler<T> vbo,
        uint plot,
        int elementCount,
        // uint stride,
        int startpointer,
        bool normalized = false
    )
    where T : unmanaged, IVertex
    {
        vbo.Bind();
        Gl.VertexAttribPointer(
            plot,
            elementCount,
            VertexAttribPointerType.Float,
            normalized,
            (uint)sizeof(T),
            startpointer
        );
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
