using System.Numerics;

namespace AvaloniaSilkOpengles.Graphics;

public interface IVertex;

public record struct VertexSingle(Vector3 Position) : IVertex;
public record struct VertexUv(Vector3 Position, Vector3 Normal, Vector2 Uv) : IVertex;
public record struct VertexColor(Vector3 Position, Vector3 Normal, Vector4 Color) : IVertex;