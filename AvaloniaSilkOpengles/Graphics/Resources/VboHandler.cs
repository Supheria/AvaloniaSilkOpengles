using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;
using AvaloniaSilkOpengles.Controls;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VboHandler<T> : ResourceHandler
    where T : unmanaged
{
    public VboHandler(
        GL gl,
        ICollection<T> data,
        BufferUsageARB usage = BufferUsageARB.StaticDraw,
        bool doUnbind = false
    ) : base(gl)
    {
        Handle = gl.GenBuffer();
        Bind();
        var array = data.ToArray();
        gl.BufferData<T>(
            BufferTargetARB.ArrayBuffer,
            (uint)(sizeof(T) * array.Length),
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
