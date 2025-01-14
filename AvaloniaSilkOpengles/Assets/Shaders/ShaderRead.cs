using System;
using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Shaders;

public class ShaderRead
{
    public static string Read(string fileName)
    {
        var uri = new Uri("avares://AvaloniaSilkOpengles/Assets/Shaders/" + fileName);
        var stream = AssetLoader.Open(uri);
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}