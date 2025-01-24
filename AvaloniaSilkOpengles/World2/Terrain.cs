using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using Avalonia;
using Avalonia.Platform;
using AvaloniaSilkOpengles.Assets;
using AvaloniaSilkOpengles.Graphics;
using LocalUtilities.SimpleScript;
using Silk.NET.OpenGLES;

namespace AvaloniaSilkOpengles.World2;

public class Terrain : GameObject
{
    public Terrain(GL gl)
    {
        var altitudeMap = LoadRawMap();
        if (altitudeMap is null)
            return;
        SetCurrentModel(gl, new TerrainModel(gl, altitudeMap));
    }

    private static AltitudeMap? LoadRawMap()
    {
        var uri = AssetsRead.GenerateUri("Test", "test altitude map.txt");
        using var stream = AssetLoader.Open(uri);
        using var memoy = new MemoryStream();
        stream.CopyTo(memoy);
        var data = memoy.ToArray();
        return SerializeTool.Deserialize<AltitudeMap>(data, 0, data.Length);
    }
}
