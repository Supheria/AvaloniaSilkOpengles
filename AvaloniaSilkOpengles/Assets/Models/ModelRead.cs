using System;
using System.IO;
using Avalonia.Platform;

namespace AvaloniaSilkOpengles.Assets.Models;

public sealed class ModelRead
{
    public string Directory { get; }

    public ModelRead(string folderName)
    {
        Directory = Path.Combine("avares://AvaloniaSilkOpengles/Assets/Models", folderName);
    }

    public Stream ReadFile(string fileName)
    {
        var uri = new Uri(Path.Combine(Directory, fileName));
        return AssetLoader.Open(uri);
    }
}
