using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Textures;

public class TextureRead : AssetsRead
{
    public static Stream Read(string textureName)
    {
        var uri = GenerateUri("Textures", textureName + ".tex");
        var stream = AssetLoader.Open(uri);
        return stream;
    }
}