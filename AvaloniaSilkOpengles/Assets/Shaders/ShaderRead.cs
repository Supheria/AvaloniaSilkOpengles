using System;
using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Shaders;

public sealed class ShaderRead : AssetsRead
{
    public static string Read(string fileName)
    {
        var uri = GenerateUri("Shaders", fileName);
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}