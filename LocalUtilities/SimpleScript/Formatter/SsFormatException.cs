namespace LocalUtilities.SimpleScript.Formatter;

internal sealed class SsFormatException(string message) : Exception(message)
{
    public static SsFormatException ElementInMultiLines(string str)
    {
        return new($"element cannot contains in multi-lines: {str}");
    }
}
