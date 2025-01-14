using System;

namespace AvaloniaSilkOpengles.Assets;

public class AssetsRead
{
    public static Uri GenerateUri(string folderName, string fileName)
    {
        var path = $"avares://AvaloniaSilkOpengles/Assets/{folderName}/{fileName}";
        return new(path);
    }
}
