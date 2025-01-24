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
    
    public void Link(VboHandler vbo)
    {
        vbo.Bind();
        LinkItem(VertexElement.Position);
        LinkItem(VertexElement.Normal);
        LinkItem(VertexElement.Color);
        LinkItem(VertexElement.Uv);
        return;

        void LinkItem(VertexElement element)
        {
            Gl.VertexAttribPointer(
                element.Plot,
                element.Count,
                VertexAttribPointerType.Float,
                false,
                (uint)sizeof(Vertex),
                element.StartPointer
            );
            Gl.EnableVertexAttribArray(element.Plot);
        }
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
