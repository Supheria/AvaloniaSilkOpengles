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
    {
        Handle = gl.GenBuffer();
        Bind(gl);
        var array = data.ToArray();
        gl.BufferData<Vertex>(
            BufferTargetARB.ArrayBuffer,
            (uint)(sizeof(Vertex) * array.Length),
            data.ToArray(),
            usage
        );
        if (doUnbind)
            Unbind(gl);
    }

    public void Bind(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, Handle);
    }

    public void Unbind(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public void Delete(GL gl)
    {
        gl.DeleteBuffer(Handle);
    }
}
