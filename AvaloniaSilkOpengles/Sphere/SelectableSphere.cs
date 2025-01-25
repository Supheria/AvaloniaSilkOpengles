using System;
using System.Collections.Generic;
using System.Drawing;
using AvaloniaSilkOpengles.Assets.Textures;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Microsoft.Xna.Framework;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.Sphere;

public class SelectableSphere : GameObject
{
    public double? IntersectsRay(Vector3 rayDirection, Vector3 rayOrigin)
    {
        var radius = Scale.X;
        var difference = Position - (rayOrigin - rayDirection);
        var differenceLengthSquared = difference.LengthSquared();
        var sphereRadiusSquared = radius * radius;
        if (differenceLengthSquared < sphereRadiusSquared)
        {
            return 0d;
        }
        var distanceAlongRay = Vector3.Dot(rayDirection, difference);
        if (distanceAlongRay < 0)
        {
            return null;
        }
        var dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;
        var result = (dist < 0) ? null : distanceAlongRay - (double?)Math.Sqrt(dist);
        return result;
    }
}
