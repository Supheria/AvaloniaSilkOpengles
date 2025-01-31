using System;
using System.Collections.Generic;
using System.Drawing;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Sphere;

public class SelectableSphere : GameObject
{
    public bool Selected { get; set; }
    public float? IntersectsRay(Vector3 rayDirection, Vector3 rayOrigin)
    {
        var bounds = new BoundingSphere(Position, Scale.X);
        var ray = new Ray(rayOrigin, rayDirection);
        return ray.Intersects(bounds);
    }
}
