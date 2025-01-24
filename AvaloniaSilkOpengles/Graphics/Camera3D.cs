using System;
using System.Diagnostics;
using System.Numerics;
using Avalonia;
using Avalonia.Input;

namespace AvaloniaSilkOpengles.Graphics;

public sealed class Camera3D
{
    float Speed { get; set; } = 2.0f;
    float Sensitivity { get; set; } = 20.0f;
    public Matrix4x4 ProjectionMatrix { get; set; }
    public Matrix4x4 ViewMatrix { get; set; }
    Vector3 Up { get; set; } = Vector3.UnitY;
    Vector3 Front { get; set; } = -Vector3.UnitZ;
    Vector3 Right { get; set; } = Vector3.UnitX;
    public Vector3 Position { get; set; }
    float PitchDegrees { get; set; }
    float YawDegrees { get; set; } = -90.0f;
    

    public Camera3D(Vector3 position)
    {
        Position = position;
    }

    public void SetSize(Size size, float fovDegrees, float nearClipPlane, float farClipPlane)
    {
        ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(fovDegrees),
            (float)(size.Width / size.Height),
            nearClipPlane,
            farClipPlane);
    }
    
    private void UpdateViewMatrix()
    {
        ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + Front, Up);
    }

    public Matrix4x4 GetMatrix()
    {
        return ViewMatrix * ProjectionMatrix;
    }

    public void UpdateControl(KeyEventArgs? keyState, Vector2 pointerPostionDiff, float timeDelta)
    {
        if (keyState is not null)
        {
            switch (keyState.Key)
            {
                case Key.W:
                    Position += Front * Speed * timeDelta;
                    UpdateViewMatrix();
                    break;
                case Key.S:
                    Position -= Front * Speed * timeDelta;
                    UpdateViewMatrix();
                    break;
                case Key.A:
                    Position -= Right * Speed * timeDelta;
                    UpdateViewMatrix();
                    break;
                case Key.D:
                    Position += Right * Speed * timeDelta;
                    UpdateViewMatrix();
                    break;
                case Key.Space:
                    Position = new(Position.X, Position.Y + Speed * timeDelta, Position.Z);
                    UpdateViewMatrix();
                    break;
                case Key.LeftShift:
                    Position = new(Position.X, Position.Y - Speed * timeDelta, Position.Z);
                    UpdateViewMatrix();
                    break;
            }
        }
        if (pointerPostionDiff != Vector2.Zero)
        {
            YawDegrees += pointerPostionDiff.X * Sensitivity * timeDelta;
            PitchDegrees -= pointerPostionDiff.Y * Sensitivity * timeDelta;
            UpdateVectors();
            UpdateViewMatrix();
        }
    }

    private void UpdateVectors()
    {
        PitchDegrees =
            PitchDegrees > 89.0f ? 89.0f
            : PitchDegrees < -89.0f ? -89.0f
            : PitchDegrees;
        var x = MathF.Cos(float.DegreesToRadians(PitchDegrees)) * MathF.Cos(float.DegreesToRadians(YawDegrees));
        var y = MathF.Sin(float.DegreesToRadians(PitchDegrees));
        var z = MathF.Cos(float.DegreesToRadians(PitchDegrees)) * MathF.Sin(float.DegreesToRadians(YawDegrees));
        Front = Vector3.Normalize(new(x, y, z));
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}
