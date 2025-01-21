namespace LocalUtilities.General;

public class PathTool
{
    public static string RenamePathByDateTime(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir is null)
            return path;
        return Path.Combine(
            dir,
            $"{Path.GetFileNameWithoutExtension(path)}_{DateTime.Now:yyyyMMddHHmmssff}.{Path.GetExtension(path)}"
        );
    }
}
