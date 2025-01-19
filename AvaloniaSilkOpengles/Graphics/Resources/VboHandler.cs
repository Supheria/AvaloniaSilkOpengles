using System.Collections.Generic;
using System.Linq;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VboHandler : ResourceHandler
{
    public VboHandler(
        GL gl,
        ICollection<Vertex> data,
        BufferUsageARB usage = BufferUsageARB.StaticDraw,
        bool doUnbind = false
    )
        : base(gl)
    {
        Handle = gl.GenBuffer();
        Bind();
        var array = data.ToArray();
        gl.BufferData<Vertex>(
            BufferTargetARB.ArrayBuffer,
            (uint)(sizeof(Vertex) * array.Length),
            data.ToArray(),
            usage
        );
        if (doUnbind)
            Unbind();
    }

    public void Bind()
    {
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);
    }

    public void Unbind()
    {
        Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public void Delete()
    {
        Gl.DeleteBuffer(Handle);
    }
}
