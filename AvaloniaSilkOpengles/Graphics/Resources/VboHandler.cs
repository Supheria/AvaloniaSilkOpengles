using System.Collections.Generic;
using System.Linq;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public sealed unsafe class VboHandler : Resource
{
    bool Dynamic { get; }
    public uint Stride { get; private set; }

    private VboHandler(uint handle, bool dynamic)
        : base(handle)
    {
        Dynamic = dynamic;
    }

    public static VboHandler Create(GL gl, bool dynamic)
    {
        var handle = gl.GenBuffer();
        var vbo = new VboHandler(handle, dynamic);
        return vbo;
    }

    public void Buffer<T>(GL gl, ICollection<T> data)
        where T : unmanaged
    {
        Bind(gl);
        var array = data.ToArray();
        var stride = sizeof(T);
        gl.BufferData<T>(
            BufferTargetARB.ArrayBuffer,
            (uint)(stride * array.Length),
            data.ToArray(),
            Dynamic ? BufferUsageARB.DynamicDraw : BufferUsageARB.StaticDraw
        );
        Stride = (uint)stride;
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
