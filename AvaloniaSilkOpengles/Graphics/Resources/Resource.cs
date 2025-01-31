using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public abstract class Resource
{
    protected uint Handle { get; }
    protected Resource(uint handle)
    {
        Handle = handle;
    }
}
