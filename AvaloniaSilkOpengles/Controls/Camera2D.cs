
// using Silk.NET.Maths;

using System.Numerics;
using Avalonia;
using Silk.NET.Maths;

namespace AvaloniaSilkOpengles.Controls;

public class Camera2D
{
    public Vector2 FocusPosition { get; set; }
    public float Zoom { get; set; }

    public Camera2D(Point focusPosition, float zoom)
    {
        FocusPosition = new((float)focusPosition.X, (float)focusPosition.Y);
        Zoom = zoom;
    }
    
    public Matrix4x4 GetProjectionMatrix(Rect bounds)
    {
        var widthHalf = (float)(bounds.Width * 0.5);
        var heightHalf = (float)(bounds.Height * 0.5);
        var left = FocusPosition.X - widthHalf;
        var right = FocusPosition.X + widthHalf;
        var top = FocusPosition.Y - heightHalf;
        var bottom = FocusPosition.Y + heightHalf;
        
        var orthoMatrix = Matrix4x4.CreateOrthographicOffCenter(left, right, top, bottom, 0f, 1f);
        var zoomMatrix = Matrix4x4.CreateScale(Zoom);
        
        return orthoMatrix * zoomMatrix;
    }
}
