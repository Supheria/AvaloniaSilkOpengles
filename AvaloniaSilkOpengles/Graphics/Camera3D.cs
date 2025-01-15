using System;
using System.Diagnostics;
using System.Numerics;
using Avalonia;
using Avalonia.Input;

namespace AvaloniaSilkOpengles.Graphics;

public sealed class Camera3D
{
    float speed { get; set; } = 0.05f;
    float sensitivity { get; set; } = 0.3f;
    float Width { get; set; }
    float Height { get; set; }
    Vector3 Up { get; set; } = Vector3.UnitY;
    Vector3 Front { get; set; } = -Vector3.UnitZ;
    Vector3 Right { get; set; } = Vector3.UnitX;
    public Vector3 Position { get; set; }
    float Pitch { get; set; }
    float Yaw { get; set; } = -90.0f;

    public Camera3D(Size size, Vector3 position)
    {
        Width = (float)size.Width;
        Height = (float)size.Height;
        Position = position;
    }
    
    public void SetSize(Size size)
    {
        Width = (float)size.Width;
        Height = (float)size.Height;
    }

    public Matrix4x4 GetViewMatrix()
    {
        return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
    }

    public Matrix4x4 GetProjectionMatrix()
    {
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(45.0f),
            Width / Height,
            0.1f,
            100.0f
        );
        return projection;
    }

    public void InputController(KeyEventArgs? keyState, Vector2 pointerPostionDiff)
    {
        if (keyState is not null)
        {
            switch (keyState.Key)
            {
                case Key.W:
                    Position += Front * speed;
                    break;
                case Key.S:
                    Position -= Front * speed;
                    break;
                case Key.A:
                    Position -= Right * speed;
                    break;
                case Key.D:
                    Position += Right * speed;
                    break;
                case Key.Space:
                    Position = new(Position.X, Position.Y + speed, Position.Z);
                    break;
                case Key.LeftShift:
                    Position = new(Position.X, Position.Y - speed, Position.Z);
                    break;
            }
        }
        if (pointerPostionDiff != Vector2.Zero)
        {
            Yaw += pointerPostionDiff.X * sensitivity;
            Pitch -= pointerPostionDiff.Y * sensitivity;
            UpdateVectors();
        }
    }

    private void UpdateVectors()
    {
        Pitch =
            Pitch > 89.0f ? 89.0f
            : Pitch < -89.0f ? -89.0f
            : Pitch;
        var x = MathF.Cos(float.DegreesToRadians(Pitch)) * MathF.Cos(float.DegreesToRadians(Yaw));
        var y = MathF.Sin(float.DegreesToRadians(Pitch));
        var z = MathF.Cos(float.DegreesToRadians(Pitch)) * MathF.Sin(float.DegreesToRadians(Yaw));
        Front = Vector3.Normalize(new(x, y, z));
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}
