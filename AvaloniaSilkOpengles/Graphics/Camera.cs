using Microsoft.Xna.Framework;

namespace AvaloniaSilkOpengles.Graphics;

public abstract class Camera
{
    public Matrix4 View { get; protected set; } = Matrix4.Identity;
    public Matrix4 Project { get; protected set; } = Matrix4.Identity;
}
