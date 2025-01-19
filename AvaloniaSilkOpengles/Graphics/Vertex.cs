using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public interface IVertex;

public record struct VertexSingle(Vector3 Postition) : IVertex;
public record struct VertexTexture(Vector3 Position, Vector3 Normal, Vector2 TexCoord) : IVertex;
public record struct VertexColor(Vector3 Position, Vector3 Normal, Vector4 Color) : IVertex;