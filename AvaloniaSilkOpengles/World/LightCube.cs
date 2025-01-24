using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using AvaloniaSilkOpengles.Graphics;
using AvaloniaSilkOpengles.Graphics.Resources;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World;

public sealed class LightCube : GameObject
{
   public LightCube(GL gl)
   {
      SetCurrentModel(gl, new LightCubeModel(gl));
   }
}
