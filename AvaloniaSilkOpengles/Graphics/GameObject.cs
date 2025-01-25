using System.Collections.Generic;
using Microsoft.Xna.Framework;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Graphics;

public class GameObject
{
    RenderableObject? ModelToRender { get; set; }
    List<Texture2DHandler> Textures { get; } = [];
    public Vector3 Scale { get; set; } = Vector3.One;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3 RotationDegrees
    {
        get => _rotationDegrees;
        set
        {
            _rotationDegrees = value;
            var rX = Matrix4.CreateRotationX(_rotationDegrees.X);
            var rY = Matrix4.CreateRotationY(_rotationDegrees.Y);
            var rZ = Matrix4.CreateRotationZ(_rotationDegrees.Z);
            var matrix = rX * rY * rZ;
            Rotation = Quaternion.CreateFromRotationMatrix(matrix);
        }
    }
    Vector3 _rotationDegrees = Vector3.Zero;
    public Vector3 Position { get; set; } = Vector3.Zero;
    public Matrix4 Matrix { get; set; } = Matrix4.Identity;
    public PrimitiveType RenderMode { get; set; } = PrimitiveType.Triangles;
    
    public void SetCurrentModel(GL gl, RenderableObject model)
    {
        ModelToRender?.Delete(gl);
        ModelToRender = model;
    }
    
    public void SetTextures(params Texture2DHandler[] textures)
    {
        Textures.Clear();
        Textures.AddRange(textures);
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
        foreach (var texture in Textures)
            texture.Delete(gl);
    }
}
