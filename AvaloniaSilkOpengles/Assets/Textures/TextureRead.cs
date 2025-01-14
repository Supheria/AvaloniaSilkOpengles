using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Textures;

public class TextureRead : AssetsRead
{
    public static Stream Read(string fileName)
    {
        var uri = GenerateUri("Textures", fileName);
        var stream = AssetLoader.Open(uri);
        return stream;
    }
}