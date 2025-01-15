using System;
using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Shaders;

public sealed class ShaderRead : AssetsRead
{
    public static string ReadVertex(string shaderName)
    {
        var uri = GenerateUri("Shaders", shaderName + ".vert");
        return Read(uri);
    }
    
    public static string ReadFragment(string shaderName)
    {
        var uri = GenerateUri("Shaders", shaderName + ".frag");
        return Read(uri);
    }
    
    private static string Read(Uri uri)
    {
        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}