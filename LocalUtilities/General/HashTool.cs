using System.Security.Cryptography;
using System.Text;

namespace LocalUtilities.General;

public static class HashTool
{
    public static string ToSha256Base64String(string input)
    {
        var data = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(data);
    }

    public static string ToSha256Base64String(FileStream file)
    {
        var sha = SHA256.Create();
        var data = sha.ComputeHash(file);
        return Convert.ToBase64String(data);
    }
}
