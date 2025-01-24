using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed class EboHandler : ResourceHandler
{
    uint ElementCount { get; }

    public EboHandler(
        GL gl,
        ICollection<uint> data,
        BufferUsageARB usage = BufferUsageARB.StaticDraw,
        bool doUnbind = false
    )
    {
        Handle = gl.GenBuffer();
        Bind(gl);
        var array = data.ToArray();
        gl.BufferData<uint>(
            BufferTargetARB.ElementArrayBuffer,
            (uint)(sizeof(uint) * array.Length),
            data.ToArray(),
            usage
        );
        ElementCount = (uint)array.Length;
        if (doUnbind)
            Unbind(gl);
    }

    public void Bind(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Handle);
    }

    public void Unbind(GL gl)
    {
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    public void Delete(GL gl)
    {
        gl.DeleteBuffer(Handle);
    }

    public unsafe void DrawElements(GL gl, PrimitiveType renderMode)
    {
        gl.DrawElements(renderMode, ElementCount, DrawElementsType.UnsignedInt, null);
    }
}
