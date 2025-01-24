using System;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VaoHandler : ResourceHandler
{
    public VaoHandler(GL gl, bool doUnbind = false)
    {
        Handle = gl.GenVertexArray();
        if (!doUnbind)
            gl.BindVertexArray(Handle);
    }

    // public void Link(
    //     VboHandler vbo,
    //     VertexElement element,
    //     bool normalized = false
    // )
    // {
    //     vbo.Bind();
    //     Gl.VertexAttribPointer(
    //         element.Plot,
    //         element.Count,
    //         VertexAttribPointerType.Float,
    //         normalized,
    //         (uint)sizeof(Vertex),
    //         element.StartPointer
    //     );
    //     Gl.EnableVertexAttribArray(element.Plot);
    // }
    
    public void Link(GL gl, VboHandler vbo)
    {
        vbo.Bind(gl);
        LinkItem(VertexElement.Position);
        LinkItem(VertexElement.Normal);
        LinkItem(VertexElement.Color);
        LinkItem(VertexElement.Uv);
        return;

        void LinkItem(VertexElement element)
        {
            gl.VertexAttribPointer(
                element.Plot,
                element.Count,
                VertexAttribPointerType.Float,
                false,
                (uint)sizeof(Vertex),
                element.StartPointer
            );
            gl.EnableVertexAttribArray(element.Plot);
        }
    }

    public void Bind(GL gl)
    {
        gl.BindVertexArray(Handle);
    }

    public void Unbind(GL gl)
    {
        gl.BindVertexArray(0);
    }

    public void Delete(GL gl)
    {
        gl.DeleteVertexArray(Handle);
    }
}
