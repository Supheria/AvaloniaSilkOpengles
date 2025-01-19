using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed class EboHandler : ResourceHandler
{
    public DrawElementsType ElementType { get; }

    public EboHandler(
        GL gl,
        ICollection<uint> data,
        BufferUsageARB usage = BufferUsageARB.StaticDraw,
        bool doUnbind = false
    ) : base(gl)
    {
        Handle = gl.GenBuffer();
        Bind();
        var array = data.ToArray();
        gl.BufferData<uint>(
            BufferTargetARB.ElementArrayBuffer,
            (uint)(sizeof(uint) * array.Length),
            data.ToArray(),
            usage
        );
        ElementType = DrawElementsType.UnsignedInt;
        if (doUnbind)
            Unbind();
    }

    public void Bind()
    {
        Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Handle);
    }

    public void Unbind()
    {
        Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    public void Delete()
    {
        Gl.DeleteBuffer(Handle);
    }
}
