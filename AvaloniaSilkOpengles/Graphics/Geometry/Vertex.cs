using System.Drawing;
using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public record struct Vertex
{
    public Position Position { get; set; }
    public Normal Normal { get; set; }
    public RenderColor Color { get; set; }
    public TexUv Uv { get; set; }
}
