using System.Collections.Generic;
using AvaloniaSilkOpengles.Graphics.Resources;
using AvaloniaSilkOpengles.World;

namespace AvaloniaSilkOpengles.Graphics;

public class Mesh
{
    List<VertexUv> Vertices {get;}
    List<uint> Indices {get;}
    List<Texture2DHandler> Textures {get;}
    VaoHandler Vao {get;set;}
}