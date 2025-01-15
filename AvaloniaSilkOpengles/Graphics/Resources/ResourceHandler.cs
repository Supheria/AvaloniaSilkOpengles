using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics.Resources;

public abstract class ResourceHandler
{
    protected GL Gl { get; }
    protected uint Handle { get; init; }
    
    protected ResourceHandler(GL gl)
    {
        Gl = gl;
    }
}
