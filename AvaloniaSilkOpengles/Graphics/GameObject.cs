using System.Collections.Generic;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public class GameObject
{
    RenderableObject? ModelToRender { get; set; }
    protected List<Texture2DHandler> Textures { get; } = [];
    public Vector3 Scale { get; set; } = Vector3.One;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3 RotationDegrees
    {
        get => _rotationDegrees;
        set
        {
            _rotationDegrees = value;
            var rX = Matrix4x4.CreateRotationX(_rotationDegrees.X);
            var rY = Matrix4x4.CreateRotationY(_rotationDegrees.Y);
            var rZ = Matrix4x4.CreateRotationZ(_rotationDegrees.Z);
            var matrix = rX * rY * rZ;
            Rotation = Quaternion.CreateFromRotationMatrix(matrix);
        }
    }
    Vector3 _rotationDegrees = Vector3.Zero;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Matrix4x4 Matrix { get; set; } = Matrix4x4.Identity;
    public PrimitiveType RenderMode { get; set; } = PrimitiveType.Triangles;
    
    protected void SetCurrentModel(GL gl, RenderableObject model)
    {
        ModelToRender?.Delete(gl);
        ModelToRender = model;
    }

    public void Render(GL gl, ShaderHandler shader, Camera3D camera)
    {
        ModelToRender?.Render(
            gl,
            RenderMode,
            Scale,
            Rotation,
            Position,
            Matrix,
            Textures,
            shader,
            camera
        );
    }
    
    public void Delete(GL gl)
    {
        ModelToRender?.Delete(gl);
    }
}
