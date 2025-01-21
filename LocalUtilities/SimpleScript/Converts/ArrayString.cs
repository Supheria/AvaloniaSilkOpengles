using System.Text;

namespace LocalUtilities.SimpleScript.Converts;

public sealed class ArrayString
{
    public static string[] ToArray(char separator, string str)
    {
        return str.Split(separator).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    }

    public static string ToArrayString(char separator, params object[] array)
    {
        return new StringBuilder().AppendJoin(separator, array).ToString();
    }
}
