using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;
using Silk.NET.OpenGLES;
using static Avalonia.OpenGL.GlConsts;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class IboHandler : Resource
{
    bool Dynamic { get; }
    uint ElementCount { get; set; }

    private IboHandler(uint handle, bool dynamic)
        : base(handle)
    {
        Dynamic = dynamic;
    }

    public static IboHandler Create(GL gl, bool dynamic)
    {
        var handle = gl.GenBuffer();
        var ibo = new IboHandler(handle, dynamic);
        return ibo;
    }

    public void Buffer(GL gl, ICollection<uint> data)
    {
        Bind(gl);
        var array = data.ToArray();
        ElementCount = (uint)array.Length;
        gl.BufferData<uint>(
            BufferTargetARB.ElementArrayBuffer,
            (uint)(sizeof(uint) * array.Length),
            data.ToArray(),
            Dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw
        );
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

    public void DrawElements(GL gl, bool wireframe)
    {
        Bind(gl);
        gl.DrawElements(
            wireframe ? PrimitiveType.Lines : PrimitiveType.Triangles,
            ElementCount,
            DrawElementsType.UnsignedInt,
            null
        );
    }
}
